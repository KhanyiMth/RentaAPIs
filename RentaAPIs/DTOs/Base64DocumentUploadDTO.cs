namespace RentaAPIs.DTOs
{
    public class Base64DocumentUploadDTO
    {
        public Guid? TenantId { get; set; }
        public Guid? PropertyId { get; set; }
        public string Category { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string FileBase64 { get; set; } = string.Empty;
    }

}
