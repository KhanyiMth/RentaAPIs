namespace RentaAPIs.Models
{
    public class MaintenanceRequest
    {
        public Guid RequestID { get; set; }
        public Guid TenantID { get; set; }
        public Guid PropertyID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public DateOnly CreatedDate { get; set; }
    }
}
