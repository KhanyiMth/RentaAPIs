using System.ComponentModel.DataAnnotations;

namespace RentaAPIs.Models
{
    public class Document
    {
        [Key]
        public Guid DocumentId { get; set; }
        public Guid? PropertyId { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? LeaseId { get; set; }
        public Guid? MaintenanceRequestId { get; set; }

        [Required]
        [MaxLength(255)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string ContentType { get; set; } = string.Empty;

        public long FileSize { get; set; }

        [Required]
        public byte[] FileData { get; set; } = Array.Empty<byte>();

        [Required]
        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public Guid? UploadedBy { get; set; }

        public Property? Property { get; set; }
        public Tenant? Tenant { get; set; }
        public Lease? Lease  { get; set; }
        public MaintenanceRequest? MaintenanceRequest { get; set; }
    }
}
