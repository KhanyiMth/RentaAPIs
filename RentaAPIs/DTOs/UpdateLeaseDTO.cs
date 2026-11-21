namespace RentaAPIs.dtos
{
    public class UpdateLeasedto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal MonthlyRent { get; set; }
        public decimal DepositAmount { get; set; }
        public string PaymentFrequency { get; set; }
        public string Status { get; set; }
        public string Terms { get; set; }
    }
}
