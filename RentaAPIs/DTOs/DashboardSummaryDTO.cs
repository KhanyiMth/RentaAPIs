namespace RentaAPIs.DTOs
{
    public class DashboardSummaryDTO
    {
        public int TotalProperties { get; set; }
        public int TotalUnits { get; set; }
        public int OccupiedUnits { get; set; }
        public int VacantUnits { get; set; }
        public int TotalTenants { get; set; }
        public int ActiveLeases { get; set; }
        public decimal MonthlyIncome { get; set; }
        public decimal OutstandingPayments { get; set; }
        public int OpenMaintenanceRequests { get; set; }
        public int UnreadNotifications { get; set; }

    }
}
