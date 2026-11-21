using System.ComponentModel.DataAnnotations;

namespace RentaAPIs.Models
{
    public class Payment 
    {
        [Key]
        public Guid PaymentId { get; set; }
        public Guid TenantID { get; set; }
        public Guid LeaseID { get; set; }

        // Total rent owed for the rental period
        public decimal Amount { get; set; }

        // Amount successfully paid
        public decimal AmountPaid { get; set; } = 0;

        // Existing payment date retained for compatibility
        public DateTime PaymentDate { get; set; }

        public DateTime PaymentDueDate { get; set; }

        // First day of the rental month
        public DateTime? RentalPeriod { get; set; }

        // Pending, Paid, PartiallyPaid, Overdue
        public string Status { get; set; } = "Pending";
        public string PaymentMethod { get; set; } = "Mock Payment";
        public string TransactionReference { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Late-payment tracking
        public bool IsLate { get; set; } = false;
        public int DaysLate { get; set; } = 0;
        public DateTime? PaidAt { get; set; }
        public Lease? Lease { get; set; }
    }
}
