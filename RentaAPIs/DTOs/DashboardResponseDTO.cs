using RentaAPIs.DTOs;

namespace RentaAPIs.dtos
{
    public class DashboardResponseDTO
    {
        public DashboardSummaryDTO DashboardSummary { get; set; }
        public List<RecentActivityDTO> RecentActivities { get; set; }
        public List<NotificationResponseDTO> Notifications { get; set; }
    }
}
