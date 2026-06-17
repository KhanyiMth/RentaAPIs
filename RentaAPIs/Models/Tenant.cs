namespace RentaAPIs.Models
{
    public class Tenant
    {
        public Guid TenantID { get; set; }
        public Guid UserID { get; set; }
        public Guid PropertyID { get; set; }
        public DateOnly MoveInDate { get; set; }
        public DateOnly MoveOutDate { get; set; }
        public string LeaseStatus { get; set; }



    }
}
