using System.ComponentModel.DataAnnotations;

namespace RentaAPIs.DTOs
{
    public class CreateMaintenanceRequestDTO
    {
        public Guid TenantId { get; set; }
        public Guid PropertyId { get; set; }
        public Guid? UnitId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        // Optional maintenance photograph
        public IFormFile? Image { get; set; }

    }
}
