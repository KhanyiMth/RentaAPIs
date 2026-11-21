using Microsoft.EntityFrameworkCore;
using RentaAPIs.cdDTOs;
using RentaAPIs.dtos;
using RentaAPIs.DTOs;

namespace RentaAPIs.Services
{
    public class DashboardService
    {
        private readonly AppDbContext _dbContext;

        public DashboardService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<DashboardResponseDTO> GetDashboard(Guid userId)
        {
            // Find landlord associated with the authenticated user.
            // Adjust the UserId comparison if your model stores it as a string.
            var landlord = await _dbContext.Landlords
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.UserId == userId.ToString());

            if (landlord == null)
            {
                throw new UnauthorizedAccessException(
                    "The authenticated user is not registered as a landlord."
                );
            }

            var landlordId = landlord.LandlordID;

            // Only leases belonging to this landlord that have been signed.
            var signedLeases = _dbContext.Leases
                .AsNoTracking()
                .Where(l =>
                    l.LandlordID == landlordId &&
                    (
                        l.Status == "SignedByTenant" ||
                        l.Status == "Active"
                    )
                );

            // Calculate monthly rental income from signed leases.
            var monthlyIncome = await signedLeases
                .SumAsync(l => (decimal?)l.MonthlyRent) ?? 0m;

            var response = new DashboardResponseDTO();

            response.DashboardSummary = new DashboardSummaryDTO
            {
                TotalProperties = await _dbContext.Properties
                    .CountAsync(p => p.LandlordId == landlordId),

                TotalUnits = await _dbContext.Units
                    .CountAsync(u =>
                        u.property.LandlordId == landlordId
                    ),

                OccupiedUnits = await _dbContext.Units
                    .CountAsync(u =>
                        u.property.LandlordId == landlordId &&
                        u.Status == "Occupied"
                    ),

                VacantUnits = await _dbContext.Units
                    .CountAsync(u =>
                        u.property.LandlordId == landlordId &&
                        u.Status == "Vacant"
                    ),

                TotalTenants = await signedLeases
                    .Select(l => l.TenantID)
                    .Distinct()
                    .CountAsync(),

                ActiveLeases = await signedLeases.CountAsync(),

                MonthlyIncome = monthlyIncome,

                OutstandingPayments = await _dbContext.Payments
                    .Where(p =>
                        p.Lease.LandlordID == landlordId &&
                        p.Status == "Pending"
                    )
                    .SumAsync(p => (decimal?)p.Amount) ?? 0m,

                OpenMaintenanceRequests = await _dbContext.MaintenanceRequests
                    .CountAsync(m =>
                        m.Property.LandlordId == landlordId &&
                        m.Status == "Open"
                    ),

                UnreadNotifications = await _dbContext.Notifications
                    .CountAsync(n =>
                        n.UserID == userId &&
                        !n.IsRead
                    )
            };

            response.Notifications = await _dbContext.Notifications
                .Where(n => n.UserID == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(5)
                .Select(n => new NotificationResponseDTO
                {
                    NotificationID = n.NotificationID,
                    UserID = n.UserID,
                    Title = n.Title,
                    Message = n.Message,
                    Type = n.Type,
                    IsRead = n.IsRead,
                    ActionUrl = n.ActionUrl,
                    CreatedAt = n.CreatedAt,
                    ReadAt = n.ReadAt
                })
                .ToListAsync();

            response.RecentActivities = new List<RecentActivityDTO>();

            return response;
        }
    }
}
