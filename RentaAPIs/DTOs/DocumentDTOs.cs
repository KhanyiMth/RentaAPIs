namespace RentaAPIs.DTOs
{
    public class CreateDocumentDTO
    {
        public Guid? PropertyID { get; set; }

        public Guid? TenantID { get; set; }

        public Guid? LeaseID { get; set; }

        public Guid? MaintenanceRequestID { get; set; }

        public string Category { get; set; } = string.Empty;

        public Guid? UploadedBy { get; set; }

        public IFormFile File { get; set; } = null!;
    }


    public class DocumentResponseDTO
    {
        public Guid DocumentID { get; set; }

        public Guid? PropertyID { get; set; }

        public Guid? TenantID { get; set; }

        public Guid? LeaseID { get; set; }

        public Guid? MaintenanceRequestID { get; set; }

        public string FileName { get; set; } = string.Empty;

        public string ContentType { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public string Category { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; }

        public Guid? UploadedBy { get; set; }

        // URL used by the React Native app to retrieve the file
        public string FileUrl { get; set; } = string.Empty;
    }

    public class DocumentFileDTO
    {
        public Guid DocumentID { get; set; }

        public string FileName { get; set; } = string.Empty;

        public string ContentType { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public byte[] FileData { get; set; } = Array.Empty<byte>();
    }
}
