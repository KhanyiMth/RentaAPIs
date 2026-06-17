namespace RentaAPIs.Models
{
    public class Lease
    {
        public Guid LeaseID { get; set; }
        public Guid TenantID { get; set; }
        public Guid PropertyID { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public decimal MonthlyRent { get; set; }
        public string LeaseDocumentURL { get; set; }
    }
}
