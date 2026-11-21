using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace RentaAPIs.DTOs
{
    public class CreateAdvertisementDTO
    {
        [Required]
        public Guid PropertyId { get; set; }

        public Guid? UnitId { get; set; }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal RentAmount { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Deposit { get; set; }

        public DateTime? ExpiryDate { get; set; }
    }
}
