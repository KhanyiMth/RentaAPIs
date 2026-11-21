namespace RentaAPIs.DTOs
{
    public class PaymentResponseDTO
    {
        public Guid PaymentID { get; set; }
        public Guid LeaseID { get; set; }
        public Guid TenantID { get; set; }
        public string TenantName { get; set; } = string.Empty;
        public string PropertyName { get; set; } = string.Empty;
        public int UnitNumber { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime PaymentDueDate { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string TransactionReference { get; set; } = string.Empty;
        public decimal AmountPaid { get; set; }
        public DateTime? RentalPeriod { get; set; }
        public bool IsLate { get; set; }
        public int DaysLate { get; set; }
        public DateTime? PaidAt { get; set; }

    }
}
