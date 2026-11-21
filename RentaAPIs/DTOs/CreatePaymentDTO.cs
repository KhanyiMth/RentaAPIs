using System.ComponentModel.DataAnnotations;

namespace RentaAPIs.DTOs
{
    public class CreatePaymentDTO
    {
        [Required]
        public Guid LeaseID { get; set; }

        [Required]
        [Range(0.01, 100000000)]
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime PaymentDueDate { get; set; }
        public string PaymentMethod { get; set; } = "Mock Payment";
    }
}
