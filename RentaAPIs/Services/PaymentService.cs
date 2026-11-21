using Microsoft.EntityFrameworkCore;
using RentaAPIs.cdDTOs;
using RentaAPIs.DTOs;
using RentaAPIs.Models;

namespace RentaAPIs.Services
{
    public class PaymentService
    {
        private readonly AppDbContext _dbContext;
        private readonly AppScoreService _appScoreService;

        public PaymentService(AppDbContext dbContext, AppScoreService appScoreService)
        {
            _dbContext = dbContext;
            _appScoreService = appScoreService;
        }

        public async Task<List<PaymentResponseDTO>> GetAllPayments()
        {
            return await _dbContext.Payments
                 .Include(p => p.Lease)
                     .ThenInclude(l => l.Tenant)
                 .Include(p => p.Lease)
                     .ThenInclude(l => l.Unit)
                         .ThenInclude(u => u.property)
                 .Select(p => new PaymentResponseDTO
                 {
                     PaymentID = p.PaymentId,

                     TenantID = p.TenantID,

                     LeaseID = p.LeaseID,

                     TenantName =
                         p.Lease.Tenant.FirstName + " " +
                         p.Lease.Tenant.Surname,

                     PropertyName =
                         p.Lease.Unit.property.Nickname,

                     UnitNumber =
                         p.Lease.Unit.UnitNumber,

                     Amount = p.Amount,

                     PaymentDate = p.PaymentDate,

                     PaymentDueDate = p.PaymentDueDate,

                     PaymentMethod = p.PaymentMethod,

                     Status = p.Status,

                     TransactionReference =
                         p.TransactionReference
                 })
                 .OrderByDescending(p => p.PaymentDate)
                 .ToListAsync();
        }


        public async Task<PaymentResponseDTO> GetPaymentById(Guid pId)
        {
            return await _dbContext.Payments
                .Include(p => p.Lease)
                    .ThenInclude(l => l.Tenant)
                .Include(p => p.Lease)
                    .ThenInclude(l => l.Unit)
                        .ThenInclude(u => u.property)
                .Where(p => p.PaymentId == pId)
                .Select(p => new PaymentResponseDTO
                {
                    PaymentID = p.PaymentId,

                    TenantID = p.TenantID,

                    LeaseID = p.LeaseID,

                    TenantName =
                        p.Lease.Tenant.FirstName + " " +
                        p.Lease.Tenant.Surname,

                    PropertyName =
                        p.Lease.Unit.property.Nickname,

                    UnitNumber =
                        p.Lease.Unit.UnitNumber,

                    Amount = p.Amount,

                    PaymentDate = p.PaymentDate,

                    PaymentDueDate = p.PaymentDueDate,

                    PaymentMethod = p.PaymentMethod,

                    Status = p.Status,

                    TransactionReference =
                        p.TransactionReference
                })
                .FirstOrDefaultAsync();

        }

        public async Task<PaymentResponseDTO> CreatePayment(CreatePaymentDTO cpDTO)
        {
            var lease = await _dbContext.Leases
                 .Include(l => l.Tenant)
                 .Include(l => l.Unit)
                     .ThenInclude(u => u.property)
                 .FirstOrDefaultAsync(l => l.LeaseId == cpDTO.LeaseID);

            if (lease == null)
                throw new Exception("Lease not found.");

            if (lease.Tenant == null)
                throw new Exception("The tenant associated with this lease could not be found.");

            var payment = new Payment
            {
                PaymentId = Guid.NewGuid(),

                TenantID = lease.TenantID,

                LeaseID = lease.LeaseId,

                Amount = cpDTO.Amount,

                PaymentDate = DateTime.UtcNow,

                PaymentDueDate =
                    cpDTO.PaymentDueDate == default
                        ? DateTime.UtcNow
                        : cpDTO.PaymentDueDate,

                PaymentMethod =
                    string.IsNullOrWhiteSpace(cpDTO.PaymentMethod)
                        ? "Mock Payment"
                        : cpDTO.PaymentMethod,

                Status = "Pending",

                TransactionReference =
                    $"RENTA-{Guid.NewGuid().ToString("N")[..12].ToUpper()}",

                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Payments.Add(payment);

            await _dbContext.SaveChangesAsync();

            return await GetPaymentById(payment.PaymentId)
                ?? throw new Exception(
                    "Unable to retrieve the newly created payment."
                );
        }

        public async Task<PaymentResponseDTO?> MockPaymentSuccess(Guid paymentId)
        {
            var payment = await _dbContext.Payments
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);

            if (payment == null)
                return null;

            if (payment.Status == "Paid")
                return await GetPaymentById(payment.PaymentId);

            payment.Status = "Paid";

            payment.PaymentDate = DateTime.UtcNow;

            if (string.IsNullOrWhiteSpace(payment.TransactionReference))
            {
                payment.TransactionReference =
                    $"RENTA-{Guid.NewGuid().ToString("N")[..12].ToUpper()}";
            }

            await _dbContext.SaveChangesAsync();

            await _appScoreService.CalculateAppScore(payment.TenantID);

            return await GetPaymentById(payment.PaymentId);
        }

        public async Task<PaymentResponseDTO?> MockPaymentFailed(Guid paymentId)
        {
            var payment = await _dbContext.Payments
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);

            if (payment == null)
                return null;

            payment.Status = "Failed";

            payment.PaymentDate = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            await _appScoreService.CalculateAppScore(payment.TenantID);

            return await GetPaymentById(payment.PaymentId);
        }


        public async Task<Payment?> MockPay(
            string userId,
            Guid leaseId,
            decimal amount,
            string paymentMethod)
        {
            var tenant = await _dbContext.Tenants
                .FirstOrDefaultAsync(t => t.UserId == userId);

            if (tenant == null)
                throw new Exception("Tenant profile not found.");

            var lease = await _dbContext.Leases
                .FirstOrDefaultAsync(l =>
                    l.LeaseId == leaseId &&
                    l.TenantID == tenant.TenantId);

            if (lease == null)
                throw new Exception("Lease not found.");

            if (amount <= 0)
                throw new Exception(
                    "Payment amount must be greater than zero.");

            // Find the oldest outstanding monthly obligation.
            // Historical records without RentalPeriod are excluded
            // until they have been reconciled.

            var today = TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow,
                TimeZoneInfo.FindSystemTimeZoneById(
                    "South Africa Standard Time")).Date;

            var payment = await _dbContext.Payments
                .Where(p =>
                        p.LeaseID == leaseId &&
                        p.RentalPeriod.HasValue &&
                        p.AmountPaid < p.Amount)
                .OrderBy(p => p.PaymentDueDate)
                .FirstOrDefaultAsync();

            if (payment == null)
                throw new Exception(
                    "No outstanding rent obligation found.");

            decimal outstanding =
                payment.Amount - payment.AmountPaid;

            if (amount > outstanding)
                throw new Exception(
                    $"Outstanding rent is R{outstanding:N2}. " +
                    "Please enter an amount within this balance.");

            var now = DateTime.UtcNow;

            payment.AmountPaid += amount;
            payment.PaymentDate = now;

            payment.PaymentMethod =
                string.IsNullOrWhiteSpace(paymentMethod)
                    ? "Mock Payment"
                    : paymentMethod;

            payment.TransactionReference =
                $"MOCK-{Guid.NewGuid():N}".Substring(0, 25)
                    .ToUpperInvariant();

            int daysLate = Math.Max(
                0,
                (today - payment.PaymentDueDate.Date).Days);

            if (daysLate > 0)
            {
                payment.IsLate = true;
                payment.DaysLate = Math.Max(
                    payment.DaysLate,
                    daysLate);
            }

            if (payment.AmountPaid >= payment.Amount)
            {
                payment.Status = "Paid";
                payment.PaidAt = now;
            }
            else
            {
                payment.Status = daysLate > 0
                    ? "Overdue"
                    : "PartiallyPaid";
            }

            await _dbContext.SaveChangesAsync();

            await _appScoreService.CalculateAppScore(
                tenant.TenantId);

            return await _dbContext.Payments
                .Include(p => p.Lease)
                .FirstOrDefaultAsync(p =>
                    p.PaymentId == payment.PaymentId);
        }


        public async Task<List<PaymentResponseDTO>> GetTenantPayments(Guid tenantId)
        {
            var payments = await _dbContext.Payments
                .AsNoTracking()
                .Where(p => p.TenantID == tenantId)
                .Select(p => new
                {
                    p.PaymentId,
                    p.TenantID,
                    p.LeaseID,

                    TenantFirstName = p.Lease != null
                        ? p.Lease.Tenant.FirstName
                        : null,

                    TenantSurname = p.Lease != null
                        ? p.Lease.Tenant.Surname
                        : null,

                    PropertyName = p.Lease != null
                        ? p.Lease.Property.Nickname
                        : null,

                    UnitNumber = p.Lease != null && p.Lease.Unit != null
                        ? (int?)p.Lease.Unit.UnitNumber
                        : null,

                    p.Amount,
                    p.AmountPaid,
                    p.RentalPeriod,
                    p.PaymentDate,
                    p.PaymentDueDate,
                    p.PaymentMethod,
                    p.Status,
                    p.TransactionReference,
                    p.IsLate,
                    p.DaysLate,
                    p.PaidAt
                })
                .ToListAsync();

            return payments
                .Select(p => new PaymentResponseDTO
                {
                    PaymentID = p.PaymentId,
                    TenantID = p.TenantID,
                    LeaseID = p.LeaseID,

                    TenantName = string.Join(" ", new[]
                    {
                p.TenantFirstName,
                p.TenantSurname
                    }.Where(x => !string.IsNullOrWhiteSpace(x))),

                    PropertyName = p.PropertyName ?? "Rental Property",

                    UnitNumber = p.UnitNumber ?? 0,

                    Amount = p.Amount,
                    AmountPaid = p.AmountPaid,
                    RentalPeriod = p.RentalPeriod,

                    PaymentDate = p.PaymentDate,
                    PaymentDueDate = p.PaymentDueDate,

                    PaymentMethod = p.PaymentMethod ?? "Mock Payment",
                    Status = p.Status ?? "Pending",
                    TransactionReference = p.TransactionReference ?? "",

                    IsLate = p.IsLate,
                    DaysLate = p.DaysLate,
                    PaidAt = p.PaidAt
                })
                .OrderByDescending(p => p.RentalPeriod ?? p.PaymentDueDate)
                .ToList();
        }
        public async Task<PaymentResponseDTO?> GetNextTenantPayment(Guid tenantId)
        {
            return await _dbContext.Payments
                .Include(p => p.Lease)
                    .ThenInclude(l => l.Tenant)
                .Include(p => p.Lease)
                    .ThenInclude(l => l.Unit)
                        .ThenInclude(u => u.property)
                .Where(p =>
                    p.TenantID == tenantId &&
                    p.Status != "Paid")
                .OrderBy(p => p.PaymentDueDate)
                .Select(p => new PaymentResponseDTO
                {
                    PaymentID = p.PaymentId,

                    TenantID = p.TenantID,

                    LeaseID = p.LeaseID,

                    TenantName =
                        p.Lease.Tenant.FirstName + " " +
                        p.Lease.Tenant.Surname,

                    PropertyName =
                        p.Lease.Unit.property.Nickname,

                    UnitNumber =
                        p.Lease.Unit.UnitNumber,

                    Amount = p.Amount,

                    PaymentDate = p.PaymentDate,

                    PaymentDueDate = p.PaymentDueDate,

                    PaymentMethod = p.PaymentMethod,

                    Status = p.Status,

                    TransactionReference =
                        p.TransactionReference
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdatePayment(Guid pId, UpdatePaymentDTO upcdDTO)
        {
            var payment = await _dbContext.Payments.FindAsync(pId);

            if (payment == null)
                return false;

            payment.Amount = upcdDTO.Amount;
            payment.PaymentDate = upcdDTO.PaymentDate;
            payment.PaymentMethod = upcdDTO.PaymentMethod;
            payment.Status = upcdDTO.Status;
            payment.TransactionReference = upcdDTO.TransactionReference;

            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeletePayment(Guid pId)
        {
            var payment = await _dbContext.Payments.FindAsync(pId);

            if (payment == null)
                return false;

            var tenantId = payment.TenantID;

            _dbContext.Payments.Remove(payment);
            await _dbContext.SaveChangesAsync();

            await _appScoreService.CalculateAppScore(tenantId);
            return true;
        }
    }
}
