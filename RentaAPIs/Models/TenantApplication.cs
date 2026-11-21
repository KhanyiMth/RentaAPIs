using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentaAPIs.Models
{
    public class TenantApplication
    {
        [Key]
        public Guid TenantApplicationId { get; set; }

        [Required]
        public Guid TenantId { get; set; }

        [Required]
        public Guid AdvertisementId { get; set; }

        [Required]
        public Guid PropertyId { get; set; }

        public Guid? UnitId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Surname { get; set; } = string.Empty;

        [Required]
        [MaxLength(250)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Cellphone { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal MonthlyIncome { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal MonthlyExpenses { get; set; }

        public int AppScore { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal AcceptanceScore { get; set; }

        [MaxLength(50)]
        public string? PredictionLabel { get; set; }

        public DateTime SubmittedDate   { get; set; }
        public DateTime UpdatedDate { get; set; }

        [MaxLength(2000)]
        public string? TenantMessage { get; set; }

        [ForeignKey(nameof(TenantId))]
        public virtual Tenant? Tenant { get; set; }

        [ForeignKey(nameof(AdvertisementId))]
        public virtual Advertisement? Advertisement { get; set; }

        [ForeignKey(nameof(PropertyId))]
        public virtual Property? Property { get; set; }

        [ForeignKey(nameof(UnitId))]
        public virtual Unit? Unit { get; set; }

    }
}
