using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RentaAPIs.dtos;
using RentaAPIs.DTOs;
using RentaAPIs.Models;

namespace RentaAPIs.Services
{
    public class TenantService
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;

        public TenantService(AppDbContext dbContext, UserManager<AppUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<List<TenantResponseDTO>> GetAllTenants()
        {
            var tenants = await _dbContext.Tenants
                .Select(t => new TenantResponseDTO
                {
                    TenantId = t.TenantId,
                    FirstName = t.FirstName,
                    Surname = t.Surname,
                    Cellphone = t.Cellphone,
                    IDNumber = t.IDNumber,
                    EmergencyContactName = t.EmergencyContactName,
                    EmergencyContactPhone = t.EmergencyContactPhone,
                    Occupation = t.Occupation,
                    EmployerName = t.EmployerName,
                    MonthlyIncome = t.MonthlyIncome,
                    MonthlyExpenses = t.MonthlyExpenses,
                    HouseholdSize = t.HouseholdSize,
                    AppScore = t.AppScore,

                }).ToListAsync();

            // Match each tenant with its ASP.NET Identity account.
            var userIds = await _dbContext.Tenants
                .Select(t => new { t.TenantId, t.UserId })
                .ToListAsync();
            var identityIds = userIds.Select(x => x.UserId).ToList();
            var users = await _userManager.Users
                .Where(u => identityIds.Contains(u.Id))
                .Select(u => new { u.Id, u.Email })
                .ToListAsync();
            var emailByUser = users.ToDictionary(u => u.Id, u => u.Email ?? "");
            var userIdByTenant = userIds.ToDictionary(x => x.TenantId, x => x.UserId);
            foreach (var dto in tenants)
            {
                if (userIdByTenant.TryGetValue(dto.TenantId, out var uid)
                    && uid != null && emailByUser.TryGetValue(uid, out var email))
                    dto.Email = email;
            }
            return tenants;
        }

        public async Task<TenantDashboardDTO> GetTenantDashboard(string userId)
        {
            var tenant = await _dbContext.Tenants
                .Include(t => t.Leases)
                    .ThenInclude(l => l.Property)
                .Include(t => t.Leases)
                    .ThenInclude(l => l.Unit)
                .FirstOrDefaultAsync(t => t.UserId == userId);

            if (tenant == null)
                throw new Exception("Tenant not found");

            var today = DateTime.UtcNow.Date;

            var signedTenantLeases = tenant.Leases
    .Where(l =>
    {
        var status = NormaliseLeaseStatus(l.Status);

        return status == "signedbytenant"
            || status == "signed"
            || status == "active";
    })
    .Where(l => l.StartDate <= today && l.EndDate >= today)
    .OrderByDescending(l => l.StartDate)
    .ToList();

            var activeLease = signedTenantLeases.FirstOrDefault();


            Console.WriteLine("==============================================");
            Console.WriteLine("TENANT DASHBOARD");
            Console.WriteLine($"Tenant ID: {tenant.TenantId}");
            Console.WriteLine($"Tenant User ID: {tenant.UserId}");
            Console.WriteLine($"Tenant Name: {tenant.FirstName} {tenant.Surname}");
            Console.WriteLine($"Total leases: {tenant.Leases.Count}");

            foreach (var lease in tenant.Leases)
            {
                Console.WriteLine(
                    $"Lease: {lease.LeaseId} | " +
                    $"Status: [{lease.Status}] | " +
                    $"Start: {lease.StartDate:yyyy-MM-dd} | " +
                    $"End: {lease.EndDate:yyyy-MM-dd}"
                );
            }

            if (activeLease != null)
            {
                Console.WriteLine(
                    $"SELECTED LEASE: {activeLease.LeaseId}"
                );

                Console.WriteLine(
                    $"SELECTED STATUS: [{activeLease.Status}]"
                );

                Console.WriteLine(
                    $"PROPERTY ID: {activeLease.PropertyID}"
                );

                Console.WriteLine(
                    $"UNIT ID: {activeLease.UnitId}"
                );
            }
            else
            {
                Console.WriteLine("NO ACTIVE SIGNED TENANT LEASE FOUND.");
            }

            Console.WriteLine("==============================================");


            var applications = await _dbContext.TenantApplications
                                    .Where(a => a.TenantId == tenant.TenantId)
                                    .Include(a => a.Advertisement)
                                        .ThenInclude(ad => ad.Property)
                                    .Include(a => a.Advertisement)
                                        .ThenInclude(ad => ad.Unit)
                                    .OrderByDescending(a => a.SubmittedDate)
                                    .ToListAsync();

            var applicationDTOs = applications.Select(a =>
            {
                var property = a.Advertisement?.Property;
                var unit = a.Advertisement?.Unit;

                var addressParts = new[]
                {
                    property?.StreetNumber,
                    property?.StreetName,
                    property?.Suburb,
                    property?.City,
                    property?.Province
                }
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();

                var address = string.Join(",", addressParts);

                return new TenantApplicationResponseDTO
                {
                    TenantApplicationId = a.TenantApplicationId,
                    TenantId = a.TenantId,
                    AdvertisementId = a.AdvertisementId,
                    PropertyId = a.PropertyId,
                    UnitId = a.UnitId,
                    PropertyTitle = a.Advertisement?.Title ?? property?.Nickname ?? "Property",
                    PropertyAddress = address,
                    UnitName = unit?.UnitName,
                    RentAmount = a.Advertisement?.RentAmount ?? 0,
                    FirstName = a.FirstName,
                    Surname = a.Surname,
                    Email = a.Email,
                    Cellphone = a.Cellphone,
                    MonthlyIncome = a.MonthlyIncome,
                    MonthlyExpenses = a.MonthlyExpenses,
                    AppScore = a.AppScore,
                    AcceptanceScore = a.AcceptanceScore,
                    PredictionLabel = a.PredictionLabel,
                    Status = a.Status,
                    TenantMessage = a.TenantMessage,
                    SubmittedDate = a.SubmittedDate,
                    UpdatedDate = a.UpdatedDate
                };
            }).ToList();

            var maintenanceRequests = await _dbContext.MaintenanceRequests
                                            .Where(m => m.TenantID == tenant.TenantId)
                                            .OrderByDescending(m => m.ReportedAt)
                                            .Select(m => new MaintenanceRequestResponseDTO
                                            {
                                                MaintenanceRequestId = m.MaintenanceReqId,
                                                TenantId = m.TenantID,
                                                TenantName = m.Tenant.FirstName + " " + m.Tenant.Surname,
                                                PropertyId = m.PropertyID,
                                                PropertyName = m.Property != null ? m.Property.Nickname : m.PropertyName,
                                                UnitId = m.UnitID,
                                                UnitNumber = m.Unit != null ? m.Unit.UnitNumber : 0,
                                                UnitName = m.Unit != null ? m.Unit.UnitName : null,
                                                Title = m.Title,
                                                Description = m.Description,
                                                Category = m.Category,
                                                Priority = m.Priority,
                                                Status = m.Status,
                                                ReportedAt = m.ReportedAt,
                                                AIPredictedCategory = m.AIPredictedCategory,
                                                AIConfidence = m.AIConfidence,
                                                AIModelVersion = m.AIModelVersion,
                                                AICategoryConfirmed = m.AICategoryConfirmed,
                                                MaintenanceImagePath = m.MaintenanceImagePath,

                                            }).ToListAsync();

            var upcomingViewings = await _dbContext.ViewingBookings
                                            .Where(v =>
                                                v.TenantId == tenant.TenantId &&
                                                v.ViewingDate >= DateTime.UtcNow)
                                            .Select(v => new TenantViewingDashboardDTO
                                            {
                                                ViewingId = v.ViewingBookingId,
                                                PropertyName = v.Property.Nickname,
                                                Date = v.ViewingDate,
                                                Time = v.StartTime.ToString(),
                                                Status = "Booked",
                                            })
                                            .OrderBy(v => v.Date)
                                            .ToListAsync();



            var dashboard = new TenantDashboardDTO
            {
                TenantName = $"{tenant.FirstName} {tenant.Surname}",

                Leases = signedTenantLeases
    .Select(l => new LeaseDashboardDTO
    {
        LeaseId = l.LeaseId,
        TenantId = l.TenantID,

        PropertyId = l.PropertyID,
        UnitId = l.UnitId,

        PropertyName = l.Property != null
            ? l.Property.Nickname
            : "Rental Property",

        UnitName = l.Unit != null
            ? l.Unit.UnitName
            : "",

        StartDate = l.StartDate,
        EndDate = l.EndDate,

        Status = l.Status,

        MonthlyRent = l.MonthlyRent,
        Deposit = l.DepositAmount
    })
    .ToList(),

                ActiveLease = activeLease == null ? null : new LeaseDashboardDTO
                {
                    LeaseId = activeLease.LeaseId,
                    TenantId = activeLease.TenantID,
                    PropertyId = activeLease.PropertyID,
                    UnitId = activeLease.UnitId,
                    PropertyName = activeLease.Property != null ? activeLease.Property.Nickname : "Rental Property",
                    UnitName = activeLease.Unit != null ? activeLease.Unit.UnitName : "",
                    StartDate = activeLease.StartDate,
                    EndDate = activeLease.EndDate,
                    Status = activeLease.Status,
                    MonthlyRent = activeLease.MonthlyRent,
                    Deposit = activeLease.DepositAmount
                },

                MaintenanceRequests = maintenanceRequests,
                UpcomingViewings = upcomingViewings,
                Applications = applicationDTOs,
                RecommendedProperties = new List<TenantPropertyDashboardDTO>(),
                SavedProperties = new List<TenantPropertyDashboardDTO>(),
                RecentlyViewedProperties = new List<TenantPropertyDashboardDTO>()
            };


            return dashboard;
        }

        private static string NormaliseLeaseStatus(string? status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return string.Empty;
            }

            return status
                .Trim()
                .ToLowerInvariant()
                .Replace(" ", "")
                .Replace("_", "")
                .Replace("-", "");
        }

        public async Task<TenantResponseDTO> GetTenantById(Guid id)
        {
            var dto = await _dbContext.Tenants
                .Where(t => t.TenantId == id)
                .Select(t => new TenantResponseDTO
                {
                    TenantId = t.TenantId,
                    FirstName = t.FirstName,
                    Surname = t.Surname,
                    Cellphone = t.Cellphone,
                    IDNumber = t.IDNumber,
                    EmergencyContactName = t.EmergencyContactName,
                    EmergencyContactPhone = t.EmergencyContactPhone,
                    Occupation = t.Occupation,
                    EmployerName = t.EmployerName,
                    MonthlyIncome = t.MonthlyIncome,
                    MonthlyExpenses = t.MonthlyExpenses,
                    HouseholdSize = t.HouseholdSize,
                    AppScore = t.AppScore,

                }).FirstOrDefaultAsync();
            if (dto == null) return null!;
            var userId = await _dbContext.Tenants
                .Where(t => t.TenantId == id)
                .Select(t => t.UserId)
                .FirstOrDefaultAsync();
            if (!string.IsNullOrEmpty(userId))
                dto.Email = (await _userManager.FindByIdAsync(userId))?.Email ?? "";
            return dto;
        }

        public async Task CreateTenant(CreateTenantDTO ctDTO)
        {
            var user = new AppUser
            {
                UserName = ctDTO.Email,
                Email = ctDTO.Email,
            };

            var result = await _userManager.CreateAsync(user, ctDTO.Password);

            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            await _userManager.AddToRoleAsync(user, "Tenant");

            var tenant = new Tenant
            {
                LandlordID = null,
                UserId = user.Id,
                FirstName = ctDTO.FirstName,
                Surname = ctDTO.Surname,
                Cellphone = ctDTO.Cellphone,
                IDNumber = ctDTO.IDNumber,
                EmergencyContactName = ctDTO.EmergencyContactName,
                EmergencyContactPhone = ctDTO.EmergencyContactPhone,
                Occupation = ctDTO.Occupation,
                EmployerName = ctDTO.EmployerName,
                MonthlyIncome = ctDTO.MonthlyIncome,
                MonthlyExpenses = ctDTO.MonthlyExpenses,
                HouseholdSize = ctDTO.HouseholdSize,

            };

            _dbContext.Tenants.Add(tenant);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<TenantResponseDTO?> GetTenantByUserId(string userId)
        {
            var dto = await _dbContext.Tenants
                .Where(t => t.UserId == userId)
                .Select(t => new TenantResponseDTO
                {
                    TenantId = t.TenantId,
                    FirstName = t.FirstName,
                    Surname = t.Surname,
                    Cellphone = t.Cellphone,
                    IDNumber = t.IDNumber,
                    EmergencyContactName = t.EmergencyContactName,
                    EmergencyContactPhone = t.EmergencyContactPhone,
                    Occupation = t.Occupation,
                    EmployerName = t.EmployerName,
                    MonthlyIncome = t.MonthlyIncome,
                    MonthlyExpenses = t.MonthlyExpenses,
                    HouseholdSize = t.HouseholdSize,
                    AppScore = t.AppScore,
                })
                .FirstOrDefaultAsync();
            if (dto != null)
                dto.Email = (await _userManager.FindByIdAsync(userId))?.Email ?? "";
            return dto;
        }

        public async Task<bool> UpdateTenant(Guid id, UpdateTenantDTO utDTO)
        {
            var tenant = await _dbContext.Tenants.FindAsync(id);

            if (tenant == null)
                return false;

            tenant.FirstName = utDTO.FirstName;
            tenant.Surname = utDTO.Surname;
            tenant.Cellphone = utDTO.Cellphone;
            tenant.EmergencyContactName = utDTO.EmergencyContactName;
            tenant.EmergencyContactPhone = utDTO.EmergencyContactPhone;
            tenant.Occupation = utDTO.Occupation;
            tenant.EmployerName = utDTO.EmployerName;
            tenant.MonthlyIncome = utDTO.MonthlyIncome;
            tenant.MonthlyExpenses = utDTO.MonthlyExpenses;
            tenant.HouseholdSize = utDTO.HouseholdSize;

            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteTenant(Guid id)
        {
            var tenant = await _dbContext.Tenants.FindAsync(id);

            if (tenant == null)
                return false;

            _dbContext.Tenants.Remove(tenant);

            return true;
        }
    }
}
