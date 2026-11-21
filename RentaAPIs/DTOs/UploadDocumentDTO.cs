using System.Globalization;

namespace RentaAPIs.DTOs
{
    public class UploadDocumentDTO
    {
        public Guid PropertyID { get; set; }
        public Guid TenantID { get; set; }
        public Guid LeaseID { get; set; }
        public Guid MaintenanceRequestID { get; set; }
        public string Category { get; set; }
        public IFormFile File { get; set; }

    }
}
