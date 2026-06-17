namespace RentaAPIs.Models
{
    public class Payment 
    {
        public Guid PaymentID { get; set; }
        public Guid TenantID { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Status { get; set; }
        public string PaymentMethod { get; set; }
    }
}
