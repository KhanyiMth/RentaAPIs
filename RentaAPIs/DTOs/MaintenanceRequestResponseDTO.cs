namespace RentaAPIs.DTOs
{
    public class MaintenanceRequestResponseDTO
    {
        public Guid MaintenanceRequestId { get; set; }
        public Guid TenantId { get; set; }
        public string TenantName { get; set; }
        public Guid PropertyId { get; set; }
        public string PropertyName { get; set; }
        public Guid? UnitId { get; set; }
        public int UnitNumber { get; set; }
        public string UnitName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public DateTime ReportedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }


        public string? AIPredictedCategory { get; set; }
        public float? AIConfidence { get; set; }
        public string? AIModelVersion { get; set; }
        public bool? AICategoryConfirmed { get; set; }
        public string? MaintenanceImagePath { get; set; }

    }
}
