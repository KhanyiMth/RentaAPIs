
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RentaAPIs.DTOs;
using RentaAPIs.Models;
using System.Text.Json;

namespace RentaAPIs.Services
{
    public class LeaseService
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;

        public LeaseService(AppDbContext dbContext, UserManager<AppUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<LeaseCreationDataDTO?> GetLeaseCreationData(
            Guid applicationId,
            string userId)
        {
            var landlord = await _dbContext.Landlords
                .FirstOrDefaultAsync(l => l.UserId == userId);

            if (landlord == null)
                return null;

            var application = await _dbContext.TenantApplications
                .FirstOrDefaultAsync(a =>
                    a.TenantApplicationId == applicationId);

            if (application == null)
                return null;

            if (!string.Equals(
                application.Status,
                "Accepted",
                StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "A lease can only be created for an accepted application.");
            }

            var property = await _dbContext.Properties
                .FirstOrDefaultAsync(p =>
                    p.PropertyId == application.PropertyId &&
                    p.LandlordId == landlord.LandlordID);

            if (property == null)
            {
                throw new UnauthorizedAccessException(
                    "This application does not belong to one of your properties.");
            }

            var tenant = await _dbContext.Tenants
                .FirstOrDefaultAsync(t =>
                    t.TenantId == application.TenantId);

            if (tenant == null)
            {
                throw new InvalidOperationException(
                    "The tenant attached to this application could not be found.");
            }

            // A unit is optional for single-occupancy properties.
            // If the application contains a UnitId, that specific unit
            // must belong to the selected property.
            Unit? unit = null;

            if (application.UnitId.HasValue)
            {
                unit = await _dbContext.Units
                    .FirstOrDefaultAsync(u =>
                        u.UnitId == application.UnitId.Value &&
                        u.PropertyId == property.PropertyId);

                if (unit == null)
                {
                    throw new InvalidOperationException(
                        "The unit attached to this application could not be found.");
                }
            }

            var propertyAddress = string.Join(", ", new[]
            {
                property.StreetNumber,
                property.StreetName,
                property.Suburb,
                property.City,
                property.Province
            }.Where(x => !string.IsNullOrWhiteSpace(x)));

            return new LeaseCreationDataDTO
            {
                ApplicationId = application.TenantApplicationId,

                TenantId = tenant.TenantId,
                TenantName = $"{tenant.FirstName} {tenant.Surname}".Trim(),
                TenantIDNumber = tenant.IDNumber,

                LandlordId = landlord.LandlordID,
                LandlordName = $"{landlord.FirstName} {landlord.Surname}".Trim(),
                LandlordIDNumber = landlord.IDNumber,
                LandlordCellphone = landlord.Cellphone,

                PropertyId = property.PropertyId,
                PropertyName = !string.IsNullOrWhiteSpace(property.Nickname)
                    ? property.Nickname
                    : "Property",
                PropertyAddress = propertyAddress,

                // Null means this is a property-level lease.
                UnitId = unit?.UnitId,
                UnitName = unit == null
                    ? string.Empty
                    : !string.IsNullOrWhiteSpace(unit.UnitName)
                        ? unit.UnitName
                        : $"Unit {unit.UnitNumber}",

                Bedrooms = unit?.Bedrooms ?? property.Bedrooms,
                Bathrooms = unit?.Bathrooms ?? property.Bathrooms,
                Parking = unit?.Parking ?? property.Parking,

                // Use the unit's amounts when a unit is selected;
                // otherwise use the property's amounts.
                MonthlyRent = unit?.RentAmount ?? property.RentAmount,
                DepositAmount = unit?.Deposit ?? property.Deposit,

                PaymentFrequency = "Monthly",
                RentDueDay = 1
            };
        }

        public async Task<LeaseResponseDTO> CreateLease(
            CreateLeaseDTO clDTO,
            string userId)
        {
            var landlord = await _dbContext.Landlords
                .FirstOrDefaultAsync(l => l.UserId == userId);

            if (landlord == null)
                throw new InvalidOperationException("Landlord profile not found.");

            var application = await _dbContext.TenantApplications
                .FirstOrDefaultAsync(a =>
                    a.TenantApplicationId == clDTO.ApplicationId);

            if (application == null)
                throw new InvalidOperationException("Application not found.");

            if (!string.Equals(
                application.Status,
                "Accepted",
                StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Only an accepted application can receive a lease.");
            }

            var property = await _dbContext.Properties
                .FirstOrDefaultAsync(p =>
                    p.PropertyId == application.PropertyId &&
                    p.LandlordId == landlord.LandlordID);

            if (property == null)
            {
                throw new UnauthorizedAccessException(
                    "This application does not belong to you.");
            }

            var tenant = await _dbContext.Tenants
                .FirstOrDefaultAsync(t =>
                    t.TenantId == application.TenantId);

            if (tenant == null)
                throw new InvalidOperationException("Tenant not found.");

            // Check for an existing lease before resolving its unit.
            var existingLease = await _dbContext.Leases
                .Include(l => l.Tenant)
                .Include(l => l.Landlord)
                .Include(l => l.Property)
                .Include(l => l.Unit)
                .FirstOrDefaultAsync(l =>
                    l.ApplicationId == application.TenantApplicationId &&
                    l.Status != "Cancelled");

            if (existingLease != null)
                return await MapLease(existingLease);

            Unit? unit = null;

            if (application.UnitId.HasValue)
            {
                unit = await _dbContext.Units
                    .FirstOrDefaultAsync(u =>
                        u.UnitId == application.UnitId.Value &&
                        u.PropertyId == property.PropertyId);

                if (unit == null)
                {
                    throw new InvalidOperationException(
                        "The unit attached to this application could not be found.");
                }
            }

            // A single-occupancy application can have no UnitId.
            // Use the property's rent and deposit in that case.
            var monthlyRent = unit?.RentAmount ?? property.RentAmount;
            var depositAmount = unit?.Deposit ?? property.Deposit;

            if (monthlyRent <= 0)
            {
                throw new InvalidOperationException(
                    "The property's or unit's monthly rent must be greater than zero.");
            }

            if (depositAmount < 0)
            {
                throw new InvalidOperationException(
                    "The property's or unit's deposit cannot be negative.");
            }

            if (clDTO.StartDate.Date < DateTime.UtcNow.Date)
            {
                throw new InvalidOperationException(
                    "The lease start date cannot be in the past.");
            }

            if (string.IsNullOrWhiteSpace(clDTO.LeaseTerm))
            {
                throw new InvalidOperationException("Lease term is required.");
            }

            var endDate = CalculateEndDate(
                clDTO.StartDate,
                clDTO.LeaseTerm);

            var dueDay = Math.Clamp(clDTO.RentDueDay, 1, 28);

            var dueDate = new DateTime(
                clDTO.StartDate.Year,
                clDTO.StartDate.Month,
                dueDay);

            var lease = new Lease
            {
                LeaseId = Guid.NewGuid(),
                ApplicationId = application.TenantApplicationId,
                TenantID = tenant.TenantId,
                LandlordID = landlord.LandlordID,
                PropertyID = property.PropertyId,

                // Requires Lease.UnitId to be Guid?.
                UnitId = unit?.UnitId,

                StartDate = clDTO.StartDate,
                EndDate = endDate,
                LeaseTerm = clDTO.LeaseTerm,
                MonthlyRent = monthlyRent,
                DepositAmount = depositAmount,
                PaymentFrequency =
                    string.IsNullOrWhiteSpace(clDTO.PaymentFrequency)
                        ? "Monthly"
                        : clDTO.PaymentFrequency,
                DueDate = dueDate,
                RentDueDay = dueDay,
                Terms = clDTO.Terms ?? string.Empty,
                UtilitiesIncluded = JsonSerializer.Serialize(
                    (clDTO.UtilitiesIncluded ?? new List<string>())
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => x.Trim())
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList()),
                PetsAllowed = clDTO.PetsAllowed,
                SpecialConditions = clDTO.SpecialConditions ?? string.Empty,
                AgreementVersion = "1.0",
                IsSignedByLandlord = false,
                IsSignedByTenant = false,
                TenantSignedAt = null,
                LandlordSignedAt = null,
                Status = "SentToTenant",
                NextPaymentDate = dueDate,
                CreatedDate = DateTime.UtcNow
            };

            _dbContext.Leases.Add(lease);
            await _dbContext.SaveChangesAsync();

            return await MapLease(lease);
        }

        private DateTime? CalculateEndDate(
            DateTime startDate,
            string leaseTerm)
        {
            var term = leaseTerm.Trim().ToLowerInvariant();

            if (term == "month-to-month" || term == "month to month")
                return null;

            var number = ExtractNumber(term);

            if (term.Contains("month"))
                return startDate.AddMonths(number).AddDays(-1);

            if (term.Contains("year"))
                return startDate.AddYears(number).AddDays(-1);

            throw new InvalidOperationException(
                "Invalid lease term. Use Month-to-month, 6 Months, 12 Months, or 24 Months.");
        }

        private int ExtractNumber(string value)
        {
            var digits = new string(value.Where(char.IsDigit).ToArray());

            if (!int.TryParse(digits, out var number) || number <= 0)
            {
                throw new InvalidOperationException(
                    "Lease term must contain a valid number.");
            }

            return number;
        }

        public async Task<LeaseResponseDTO?> GetLeaseById(
            Guid id,
            string userId)
        {
            var landlord = await _dbContext.Landlords
                .FirstOrDefaultAsync(l => l.UserId == userId);

            var tenant = await _dbContext.Tenants
                .FirstOrDefaultAsync(t => t.UserId == userId);

            var lease = await _dbContext.Leases
                .Include(l => l.Tenant)
                .Include(l => l.Property)
                .Include(l => l.Unit)
                .Include(l => l.Landlord)
                .FirstOrDefaultAsync(l => l.LeaseId == id);

            if (lease == null)
                return null;

            var landlordAllowed =
                landlord != null &&
                lease.LandlordID == landlord.LandlordID;

            var tenantAllowed =
                tenant != null &&
                lease.TenantID == tenant.TenantId;

            if (!landlordAllowed && !tenantAllowed)
            {
                throw new UnauthorizedAccessException(
                    "You are not authorised to view this lease.");
            }

            return await MapLease(lease);
        }


        public async Task<LeaseResponseDTO?> GetLeaseByApplication(
            Guid applicationId,
            string userId)
        {
            var landlord = await _dbContext.Landlords
                .FirstOrDefaultAsync(l => l.UserId == userId);

            var tenant = await _dbContext.Tenants
                .FirstOrDefaultAsync(t => t.UserId == userId);

            if (landlord == null && tenant == null)
            {
                throw new UnauthorizedAccessException(
                    "No landlord or tenant profile was found for this user.");
            }

            var lease = await _dbContext.Leases
                .Include(l => l.Tenant)
                .Include(l => l.Landlord)
                .Include(l => l.Property)
                .Include(l => l.Unit)
                .FirstOrDefaultAsync(l =>
                    l.ApplicationId == applicationId);

            if (lease == null)
            {
                return null;
            }

            var isLandlord =
                landlord != null &&
                lease.LandlordID == landlord.LandlordID;

            var isTenant =
                tenant != null &&
                lease.TenantID == tenant.TenantId;

            if (!isLandlord && !isTenant)
            {
                throw new UnauthorizedAccessException(
                    "You are not authorised to view this lease.");
            }

            return await MapLease(lease);
        }



        public async Task<LeaseResponseDTO> SignLease(
            Guid leaseId,
            SignLeaseDTO slDTO,
            string userId)
        {
            var tenant = await _dbContext.Tenants
                .FirstOrDefaultAsync(t => t.UserId == userId);

            if (tenant == null)
            {
                throw new UnauthorizedAccessException(
                    "Tenant profile not found.");
            }

            if (slDTO == null ||
                string.IsNullOrWhiteSpace(slDTO.Signature))
            {
                throw new InvalidOperationException(
                    "A tenant signature is required.");
            }

            var lease = await _dbContext.Leases
                .Include(l => l.Tenant)
                .Include(l => l.Landlord)
                .Include(l => l.Property)
                .Include(l => l.Unit)
                .FirstOrDefaultAsync(l => l.LeaseId == leaseId);

            if (lease == null)
            {
                throw new InvalidOperationException("Lease not found.");
            }

            if (lease.TenantID != tenant.TenantId)
            {
                throw new UnauthorizedAccessException(
                    "You are not authorised to sign this lease.");
            }

            if (!string.Equals(
                lease.Status,
                "SentToTenant",
                StringComparison.OrdinalIgnoreCase))
            {
                if (lease.IsSignedByTenant)
                {
                    throw new InvalidOperationException(
                        "This lease has already been signed by the tenant.");
                }

                throw new InvalidOperationException(
                    $"This lease cannot be signed while it is in '{lease.Status}' status.");
            }

            // 1. Sign the lease
            lease.TenantSignature = slDTO.Signature;
            lease.IsSignedByTenant = true;
            lease.TenantSignedAt = DateTime.UtcNow;
            lease.ReturnedToLandlordAt = DateTime.UtcNow;
            lease.Status = "SignedByTenant";

            // 2. Deactivate the matching advertisements
            // For multi-unit properties, only deactivate
            // advertisements for the leased unit.
            // For single-occupancy properties, deactivate
            // the property-level advertisements.

            var advertisements = await _dbContext.Advertisements
    .Where(a =>
        a.PropertyId == lease.PropertyID &&
        a.UnitId == lease.UnitId)
    .ToListAsync();

            foreach (var advertisement in advertisements)
            {
                advertisement.Status = "Inactive";
            }

            // 3. Notify the landlord
            var landlord = lease.Landlord ??
                await _dbContext.Landlords
                    .FirstOrDefaultAsync(l =>
                        l.LandlordID == lease.LandlordID);

            if (landlord == null)
            {
                throw new InvalidOperationException(
                    "The landlord attached to this lease could not be found.");
            }

            var propertyName = lease.Property?.Nickname;

            if (string.IsNullOrWhiteSpace(propertyName))
                propertyName = "the property";

            var tenantName = lease.Tenant == null
                ? "The tenant"
                : $"{lease.Tenant.FirstName} {lease.Tenant.Surname}".Trim();

            var landlordNotification = new Notification
            {
                NotificationID = Guid.NewGuid(),
                LandlordID = landlord.LandlordID,
                UserID = landlord.LandlordID,
                Title = "Lease signed by Tenant",
                Message = $"{tenantName} has signed the lease for {propertyName}. The lease has been returned to you.",
                Type = "LeaseSigned",
                IsRead = false,
                ActionUrl = $"/lease/{lease.LeaseId}",
                CreatedAt = DateTime.UtcNow,
            };

            _dbContext.Notifications.Add(landlordNotification);

            // 4. Save lease, advertisement status
            // and notification together
            await _dbContext.SaveChangesAsync();

            return await MapLease(lease);
        }


        private async Task<LeaseResponseDTO> MapLease(Lease lease)
        {
            var tenant = lease.Tenant ??
                await _dbContext.Tenants
                    .FirstOrDefaultAsync(t =>
                        t.TenantId == lease.TenantID);

            var landlord = lease.Landlord ??
                await _dbContext.Landlords
                    .FirstOrDefaultAsync(l =>
                        l.LandlordID == lease.LandlordID);

            var property = lease.Property ??
                await _dbContext.Properties
                    .FirstOrDefaultAsync(p =>
                        p.PropertyId == lease.PropertyID);

            Unit? unit = lease.Unit;

            if (unit == null && lease.UnitId.HasValue)
            {
                unit = await _dbContext.Units
                    .FirstOrDefaultAsync(u =>
                        u.UnitId == lease.UnitId.Value);
            }

            var propertyAddress = property == null
                ? string.Empty
                : string.Join(", ", new[]
                {
                    property.StreetNumber,
                    property.StreetName,
                    property.Suburb,
                    property.City,
                    property.Province
                }.Where(x => !string.IsNullOrWhiteSpace(x)));

            return new LeaseResponseDTO
            {
                LeaseId = lease.LeaseId,
                ApplicationId = lease.ApplicationId,
                TenantId = lease.TenantID,
                TenantName = tenant == null
                    ? string.Empty
                    : $"{tenant.FirstName} {tenant.Surname}".Trim(),
                TenantIDNumber = tenant?.IDNumber ?? string.Empty,
                TenantAddress = tenant?.Address ?? string.Empty,

                LandlordId = lease.LandlordID,
                LandlordName = landlord == null
                    ? string.Empty
                    : $"{landlord.FirstName} {landlord.Surname}".Trim(),
                LandlordIDNumber = landlord?.IDNumber ?? string.Empty,
                LandlordAddress = landlord?.Address ?? string.Empty,

                PropertyId = lease.PropertyID,
                PropertyName = property?.Nickname ?? "Property",
                PropertyAddress = propertyAddress,

                // No unit is expected for a single-occupancy property.
                UnitId = lease.UnitId,
                UnitName = unit == null
                    ? string.Empty
                    : !string.IsNullOrWhiteSpace(unit.UnitName)
                        ? unit.UnitName
                        : $"Unit {unit.UnitNumber}",
                UnitNumber = unit?.UnitNumber ?? 0,

                StartDate = lease.StartDate,
                EndDate = lease.EndDate,
                LeaseTerm = lease.LeaseTerm,
                MonthlyRent = lease.MonthlyRent,
                DepositAmount = lease.DepositAmount,
                PaymentFrequency = lease.PaymentFrequency,
                DueDate = lease.DueDate,
                RentDueDay = lease.RentDueDay,
                Terms = lease.Terms,
                UtilitiesIncluded = ParseUtilities(lease.UtilitiesIncluded),
                PetsAllowed = lease.PetsAllowed,
                SpecialConditions = lease.SpecialConditions,
                AgreementVersion = lease.AgreementVersion,
                Status = lease.Status,
                IsSignedByTenant = lease.IsSignedByTenant,
                IsSignedByLandlord = lease.IsSignedByLandlord,
                TenantSignedAt = lease.TenantSignedAt,
                LandlordSignedAt = lease.LandlordSignedAt,
                ReturnedToLandlordAt = lease.ReturnedToLandlordAt,
                CreatedDate = lease.CreatedDate
            };
        }

        private static List<string> ParseUtilities(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new List<string>();

            try
            {
                return JsonSerializer.Deserialize<List<string>>(json)
                    ?? new List<string>();
            }
            catch (JsonException)
            {
                // Older leases may have stored utilities as plain text.
                return new List<string>();
            }
        }

        public async Task<List<LeaseResponseDTO>> GetAllLeases()
        {
            var leases = await _dbContext.Leases
                .Include(l => l.Tenant)
                .Include(l => l.Landlord)
                .Include(l => l.Property)
                .Include(l => l.Unit)
                .ToListAsync();

            var results = new List<LeaseResponseDTO>();

            foreach (var lease in leases)
            {
                results.Add(await MapLease(lease));
            }

            return results;
        }

        public async Task<bool> DeleteLease(Guid id, string userId)
        {
            var landlord = await _dbContext.Landlords
                .FirstOrDefaultAsync(l => l.UserId == userId);

            if (landlord == null)
                return false;

            var lease = await _dbContext.Leases
                .FirstOrDefaultAsync(l =>
                    l.LeaseId == id &&
                    l.LandlordID == landlord.LandlordID);

            if (lease == null)
                return false;

            _dbContext.Leases.Remove(lease);

            await _dbContext.SaveChangesAsync();

            return true;
        }
    }

}
