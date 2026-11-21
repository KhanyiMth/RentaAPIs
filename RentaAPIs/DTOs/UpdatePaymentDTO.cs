using System.ComponentModel.DataAnnotations;
using System.Net;

namespace RentaAPIs.DTOs
{
    public class UpdatePaymentDTO
    {
        [Range(0.01, 100000000)]
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; } = "Mock Payment";
        public string Status { get; set; } = "Pending";
        public string TransactionReference { get; set; } = string.Empty;

    }
}
