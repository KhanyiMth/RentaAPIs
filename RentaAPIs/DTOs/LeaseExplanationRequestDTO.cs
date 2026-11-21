using System.ComponentModel.DataAnnotations;

namespace RentaAPIs.DTOs
{
    public class LeaseExplanationRequestDTO
    {
        [Required]
        [StringLength(20000, MinimumLength = 10)]
        public string LeaseText { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Question { get; set; }
    }
}
