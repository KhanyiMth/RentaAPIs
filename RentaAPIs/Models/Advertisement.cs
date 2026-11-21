using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentaAPIs.Models
{
    public class Advertisement
    {
        [Key]
        public Guid AdvertisementId { get; set; }

        public Guid PropertyId { get; set; }
        public Guid? UnitId { get; set; }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal RentAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Deposit { get; set; }

        [Required]
        [MaxLength(30)]
        public string Status { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ExpiryDate { get; set; } 

        [ForeignKey(nameof(PropertyId))]
        public Property Property { get; set; }

        [ForeignKey(nameof(UnitId))]
        public Unit? Unit { get; set; }
    }
}
