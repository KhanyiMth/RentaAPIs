using System.ComponentModel.DataAnnotations;

namespace RentaAPIs.Models
{
    public class MaintenanceRequest
    {
        [Key]
        public Guid MaintenanceReqId { get; set; }

        public Guid TenantID { get; set; }
        public Guid PropertyID { get; set; }
        public Guid? UnitID { get; set; }

        public string Title { get; set; } = string.Empty;
        public string PropertyName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Priority { get; set; } = "Medium";
        public string Status { get; set; } = "Open";

        public DateTime ReportedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public DateOnly CreatedDate { get; set; }

        // RentaVision: optional AI assistance
        public string? AIPredictedCategory { get; set; }
        public float? AIConfidence { get; set; }
        public string? AIModelVersion { get; set; }
        public bool? AICategoryConfirmed { get; set; }

        // Image reference, not image binary
        public string? MaintenanceImagePath { get; set; }

        public Tenant Tenant { get; set; } = null!;
        public Property Property { get; set; } = null!;
        public Unit? Unit { get; set; }

        public ICollection<Document> Documents { get; set; }
            = new List<Document>();
    }
}
