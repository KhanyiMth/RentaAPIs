using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace RentaAPIs.Models
{
    public class Lease
    {
        [Key]
        public Guid LeaseId { get; set; }
        public Guid ApplicationId { get; set; }
        public Guid TenantID { get; set; }
        public Guid LandlordID { get; set; }
        public Guid PropertyID { get; set; }
        public Guid? UnitId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string LeaseTerm { get; set; } = string.Empty;
        public decimal MonthlyRent { get; set; }
        public decimal DepositAmount { get; set; }
        public string PaymentFrequency { get; set; } = "Monthly";
        public DateTime DueDate { get; set; }
        public string Terms { get; set; } = string.Empty;
        public bool IsSignedByLandlord { get; set; }
        public bool IsSignedByTenant { get; set; }
        public DateTime? TenantSignedAt { get; set; }
        public DateTime? LandlordSignedAt { get; set; }
        public string? TenantSignature { get; set; }
        public DateTime? ReturnedToLandlordAt { get; set; }
        public string Status { get; set; } = "Draft";
        public DateTime? NextPaymentDate { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string? LeaseDocumentURL { get; set; } = null;
        public int RentDueDay { get; set; } = 1;
        public string UtilitiesIncluded { get; set; } = "[]";
        public bool PetsAllowed { get; set; } = false;
        public string SpecialConditions { get; set; } = string.Empty;
        public string AgreementVersion { get; set; } = "1.0";

        public Tenant Tenant { get; set; } = null!;
        public Property Property { get; set; } = null!;
        public Landlord Landlord { get; set; } = null!;
        public Unit? Unit { get; set; }
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}
