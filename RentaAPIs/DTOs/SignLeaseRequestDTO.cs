using System.ComponentModel.DataAnnotations;

namespace RentaAPIs.DTOs
{
    public class SignLeaseRequestDTO
    {
        [Required]
        public Guid LeaseId { get; set; }

        [Required]
        public string Signature { get; set; } = string.Empty;

    }
}
