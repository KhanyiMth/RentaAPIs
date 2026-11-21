using RentaAPIs.DTOs;

namespace RentaAPIs.dtos
{
    public class TenantDashboardDTO
    {
        public string TenantName { get; set; }
        public bool HasActiveLease { get; set; }
        public LeaseDashboardDTO? ActiveLease { get; set; }
        public PaymentDashboardDTO NextPayment { get; set; }
        public List<MaintenanceRequestResponseDTO> MaintenanceRequests { get; set; }
        public List<TenantApplicationResponseDTO> Applications { get; set; } = new List<TenantApplicationResponseDTO>();
        public List<TenantPropertyDashboardDTO> RecommendedProperties { get; set; }
        public List<TenantPropertyDashboardDTO> SavedProperties { get; set; }
        public List<TenantViewingDashboardDTO> UpcomingViewings { get; set; }
        public List<TenantPropertyDashboardDTO> RecentlyViewedProperties { get; set; }
        public List<LeaseDashboardDTO> Leases { get; set; } = new();
    }

    public class LeaseDashboardDTO
    {
        public Guid LeaseId { get; set; }
        public Guid PropertyId { get; set; }
        public Guid TenantId { get; set; }
        public Guid? UnitId { get; set; }
        public string PropertyName { get; set; }
        public string UnitName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; }
        public decimal MonthlyRent { get; set; }
        public decimal Deposit { get; set; }
    }

    public class PaymentDashboardDTO
    {
        public Guid? PaymentId { get; set; }
        public decimal Amount { get; set; }
        public DateTime? DueDate { get; set; }
        public string Status { get; set; }
    }

    public class TenantApplicationDashboardDTO
    {
        public Guid ApplicationId { get; set; }
        public string PropertyName { get; set; }
        public string Status { get; set; }
    }

    public class TenantPropertyDashboardDTO
    {
        public Guid PropertyId { get; set; }
        public string Nickname { get; set; }
        public string Title { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Suburb { get; set; }
        public decimal RentAmount { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
    }

    public class TenantViewingDashboardDTO
    {
        public Guid ViewingId { get; set; }
        public string PropertyName { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public string Status { get; set; }
    }

}
