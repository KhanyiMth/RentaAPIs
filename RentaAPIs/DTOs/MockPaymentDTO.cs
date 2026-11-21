namespace RentaAPIs.DTOs
{
    public class MockPaymentDTO
    {

        public Guid LeaseId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = "Card";

    }
}
