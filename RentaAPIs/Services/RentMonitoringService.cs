using Microsoft.EntityFrameworkCore;
using RentaAPIs.Models;

namespace RentaAPIs.Services
{
    public class RentMonitoringService
    {
        private readonly AppDbContext _dbContext;
        private readonly AppScoreService _appScoreService;

        private static readonly TimeZoneInfo SaTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById(
                "South Africa Standard Time");

        public RentMonitoringService(
            AppDbContext dbContext,
            AppScoreService appScoreService)
        {
            _dbContext = dbContext;
            _appScoreService = appScoreService;
        }

        private static DateTime GetToday()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow,
                SaTimeZone).Date;
        }

        public async Task<int> MonitorRentPayments()
        {
            var today = GetToday();

            // Start from October to avoid incorrectly
            // recreating September's historical payments.
            var monitoringStart =
                new DateTime(2026, 10, 1);

            var leases = await _dbContext.Leases
    .Where(l =>
        l.Status == "SignedByTenant" &&
        l.IsSignedByTenant &&
        l.StartDate <= today &&
        (!l.EndDate.HasValue ||
         l.EndDate.Value >= monitoringStart))
    .ToListAsync();

            int changes = 0;
            var affectedTenants = new HashSet<Guid>();

            foreach (var lease in leases)
            {
                var firstMonth = new DateTime(
                    Math.Max(
                        lease.StartDate.Year,
                        monitoringStart.Year),
                    1, 1);

                // Begin with the later of the lease start
                // month and the monitoring start month.
                var leaseStartMonth = new DateTime(
                    lease.StartDate.Year,
                    lease.StartDate.Month, 1);

                var period =
                    leaseStartMonth > monitoringStart
                        ? leaseStartMonth
                        : monitoringStart;

                while (period <= today)
                {
                    if (lease.EndDate.HasValue &&
                        period > lease.EndDate.Value.Date)
                    {
                        break;
                    }

                    int dueDay = Math.Clamp(
                        lease.RentDueDay, 1,
                        DateTime.DaysInMonth(
                            period.Year, period.Month));

                    var dueDate = new DateTime(
                        period.Year,
                        period.Month,
                        dueDay);

                    var payment = await _dbContext.Payments
                        .FirstOrDefaultAsync(p =>
                            p.LeaseID == lease.LeaseId &&
                            p.RentalPeriod == period);

                    if (payment == null)
                    {
                        payment = new Payment
                        {
                            PaymentId = Guid.NewGuid(),
                            TenantID = lease.TenantID,
                            LeaseID = lease.LeaseId,
                            Amount = lease.MonthlyRent,
                            AmountPaid = 0,
                            RentalPeriod = period,
                            PaymentDueDate = dueDate,
                            PaymentDate = DateTime.MinValue,
                            Status = today > dueDate
                                ? "Overdue"
                                : "Pending",
                            IsLate = today > dueDate,
                            DaysLate = Math.Max(
                                0, (today - dueDate).Days),
                            CreatedAt = DateTime.UtcNow
                        };

                        _dbContext.Payments.Add(payment);

                        changes++;
                        affectedTenants.Add(lease.TenantID);
                    }
                    else if (payment.Status != "Paid")
                    {
                        decimal outstanding =
                            payment.Amount -
                            payment.AmountPaid;

                        bool overdue =
                            today > dueDate &&
                            outstanding > 0;

                        string newStatus =
                            outstanding <= 0
                                ? "Paid"
                                : overdue
                                    ? "Overdue"
                                    : payment.AmountPaid > 0
                                        ? "PartiallyPaid"
                                        : "Pending";

                        int daysLate = overdue
                            ? (today - dueDate).Days
                            : 0;

                        if (payment.Status != newStatus ||
                            payment.DaysLate != daysLate ||
                            payment.IsLate != overdue)
                        {
                            payment.Status = newStatus;
                            payment.DaysLate = daysLate;
                            payment.IsLate = overdue;

                            changes++;
                            affectedTenants.Add(lease.TenantID);
                        }
                    }

                    period = period.AddMonths(1);
                }
            }

            await _dbContext.SaveChangesAsync();

            foreach (var tenantId in affectedTenants)
            {
                await _appScoreService
                    .CalculateAppScore(tenantId);
            }

            return changes;
        }
    }
}
