using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using RentaAPIs.DTOs;
using RentaAPIs.Models;
using System.Reflection.Metadata;
using System.Xml.Linq;
using Document = RentaAPIs.Models.Document;

namespace RentaAPIs.Services
{
    public class DocumentService
    {
        public AppDbContext _dbContext;
        private const long MaxFileSize = 10 * 1024 * 1024;

        private static readonly string[] AllowedContentTypes =
        {
            "application/pdf",
            "image/jpeg",
            "image/png",
            "image/webp"
        };

        private static readonly string[] AllowedExtensions =
        {
            ".pdf",
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        };

        public DocumentService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task <DocumentResponseDTO> UploadDocument (CreateDocumentDTO dto)
        {
            if (dto.File == null || dto.File.Length == 0)
            {
                throw new Exception("No file was supplied.");
            }

            if (dto.File.Length > MaxFileSize)
            {
                throw new Exception(
                    "File is too large. Maximum file size is 10 MB.");
            }

            if (!AllowedContentTypes.Contains(
                dto.File.ContentType.ToLower()))
            {
                throw new Exception(
                    "Unsupported file type. Only PDF, JPG, PNG and WEBP files are allowed.");
            }

            if (string.IsNullOrWhiteSpace(dto.Category))
            {
                throw new Exception("Document category is required.");
            }

            using var memoryStream = new MemoryStream();

            await dto.File.CopyToAsync(memoryStream);

            var document = new Document
            {
                DocumentId = Guid.NewGuid(),

                PropertyId = dto.PropertyID,
                TenantId = dto.TenantID,
                LeaseId = dto.LeaseID,
                MaintenanceRequestId = dto.MaintenanceRequestID,

                FileName = Path.GetFileName(dto.File.FileName),

                ContentType = dto.File.ContentType,

                FileSize = dto.File.Length,

                FileData = memoryStream.ToArray(),

                Category = dto.Category.Trim(),

                UploadedAt = DateTime.UtcNow,

                UploadedBy = dto.UploadedBy
            };

            _dbContext.Documents.Add(document);

            await _dbContext.SaveChangesAsync();

            return MapToResponse(document);
        }


        public async Task <DocumentResponseDTO?> GetDocumentById (Guid documentId)
        {
            var document = await _dbContext.Documents
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.DocumentId == documentId);

            if (document == null)
            {
                return null;
            }

            return MapToResponse(document);
        }

        public async Task <DocumentFileDTO?> GetDocumentFile (Guid documentId)
        {
            var document = await _dbContext.Documents
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.DocumentId == documentId);

            if (document == null)
            {
                return null;
            }

            return new DocumentFileDTO
            {
                DocumentID = document.DocumentId,

                FileName = document.FileName,

                ContentType = document.ContentType,

                FileSize = document.FileSize,

                FileData = document.FileData
            };
        }

        public async Task<List<DocumentResponseDTO?>> GetTenantDocuments (Guid tenantId)
        {
            var documents = await _dbContext.Documents
                .AsNoTracking()
                .Where(x =>
                    x.TenantId == tenantId)
                .OrderByDescending(x => x.UploadedAt)
                .Select(x => new DocumentResponseDTO
                {
                    DocumentID = x.DocumentId,

                    PropertyID = x.PropertyId,

                    TenantID = x.TenantId,

                    LeaseID = x.LeaseId,

                    MaintenanceRequestID =
                        x.MaintenanceRequestId,

                    FileName = x.FileName,

                    ContentType = x.ContentType,

                    FileSize = x.FileSize,

                    Category = x.Category,

                    UploadedAt = x.UploadedAt,

                    UploadedBy = x.UploadedBy,

                    FileUrl =
                        $"/api/Document/file/{x.DocumentId}"
                })
                .ToListAsync();

            return documents;
        }

        public async Task<DocumentResponseDTO> UploadBase64Document( Base64DocumentUploadDTO dto, Guid? uploadedBy = null)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            // ---------------------------------------------------------
            // 1. Validate that the upload belongs to a tenant OR property
            // ---------------------------------------------------------
            if (!dto.TenantId.HasValue && !dto.PropertyId.HasValue)
            {
                throw new Exception(
                    "A TenantId or PropertyId is required for this document upload."
                );
            }

            // ---------------------------------------------------------
            // 2. Validate Tenant if TenantId was supplied
            // ---------------------------------------------------------
            if (dto.TenantId.HasValue)
            {
                var tenantExists = await _dbContext.Tenants
                    .AnyAsync(t => t.TenantId == dto.TenantId.Value);

                if (!tenantExists)
                {
                    throw new Exception("Tenant could not be found.");
                }
            }

            // ---------------------------------------------------------
            // 3. Validate Property if PropertyId was supplied
            // ---------------------------------------------------------
            if (dto.PropertyId.HasValue)
            {
                var propertyExists = await _dbContext.Properties
                    .AnyAsync(p => p.PropertyId == dto.PropertyId.Value);

                if (!propertyExists)
                {
                    throw new Exception("Property could not be found.");
                }
            }

            // ---------------------------------------------------------
            // 4. Validate required file information
            // ---------------------------------------------------------
            if (string.IsNullOrWhiteSpace(dto.FileName))
            {
                throw new Exception("File name is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.ContentType))
            {
                throw new Exception("Content type is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.FileBase64))
            {
                throw new Exception("File data is required.");
            }

            // ---------------------------------------------------------
            // 5. Validate file type
            // ---------------------------------------------------------
            var allowedTypes = new[]
            {
        "application/pdf",
        "image/jpeg",
        "image/jpg",
        "image/png",
        "image/webp"
    };

            if (!allowedTypes.Contains(
                    dto.ContentType.Trim().ToLowerInvariant()))
            {
                throw new Exception(
                    "Unsupported file type. Only PDF, JPG, PNG and WEBP files are allowed."
                );
            }

            // ---------------------------------------------------------
            // 6. Remove possible Base64 data URL prefix
            // ---------------------------------------------------------
            var base64Data = dto.FileBase64;

            if (base64Data.Contains(","))
            {
                base64Data = base64Data.Substring(
                    base64Data.IndexOf(",") + 1
                );
            }

            // ---------------------------------------------------------
            // 7. Convert Base64 to bytes
            // ---------------------------------------------------------
            byte[] fileBytes;

            try
            {
                fileBytes = Convert.FromBase64String(base64Data);
            }
            catch (FormatException)
            {
                throw new Exception("The supplied file data is not valid Base64.");
            }

            // ---------------------------------------------------------
            // 8. Validate file size
            // Maximum = 10 MB
            // ---------------------------------------------------------
            const long maxFileSize = 10 * 1024 * 1024;

            if (fileBytes.Length > maxFileSize)
            {
                throw new Exception("File size cannot exceed 10 MB.");
            }

            if (fileBytes.Length == 0)
            {
                throw new Exception("The uploaded file is empty.");
            }

            // ---------------------------------------------------------
            // 9. Create Document entity
            // ---------------------------------------------------------
            var document = new Document
            {
                DocumentId = Guid.NewGuid(),

                // Tenant document OR property image
                TenantId = dto.TenantId,
                PropertyId = dto.PropertyId,

                Category = dto.Category?.Trim() ?? string.Empty,

                FileName = dto.FileName.Trim(),

                ContentType = dto.ContentType.Trim().ToLowerInvariant(),

                FileData = fileBytes,

                UploadedAt = DateTime.UtcNow,

                UploadedBy = uploadedBy
            };

            // ---------------------------------------------------------
            // 10. Save document
            // ---------------------------------------------------------
            _dbContext.Documents.Add(document);

            await _dbContext.SaveChangesAsync();

            // ---------------------------------------------------------
            // 11. Return saved document
            // ---------------------------------------------------------
            return MapToResponse(document);
        }

        private static void ValidateFileSignature(
            byte[] bytes,
            string extension)
        {
            if (bytes.Length < 4)
            {
                throw new Exception("The uploaded document is invalid or empty.");
            }

            bool valid = extension switch
            {
                ".pdf" =>
                    bytes.Length >= 5 &&
                    bytes[0] == 0x25 &&
                    bytes[1] == 0x50 &&
                    bytes[2] == 0x44 &&
                    bytes[3] == 0x46 &&
                    bytes[4] == 0x2D,

                ".jpg" or ".jpeg" =>
                    bytes.Length >= 3 &&
                    bytes[0] == 0xFF &&
                    bytes[1] == 0xD8 &&
                    bytes[2] == 0xFF,

                ".png" =>
                    bytes.Length >= 8 &&
                    bytes[0] == 0x89 &&
                    bytes[1] == 0x50 &&
                    bytes[2] == 0x4E &&
                    bytes[3] == 0x47 &&
                    bytes[4] == 0x0D &&
                    bytes[5] == 0x0A &&
                    bytes[6] == 0x1A &&
                    bytes[7] == 0x0A,

                // WEBP files begin with RIFF and contain WEBP at offset 8.
                ".webp" =>
                    bytes.Length >= 12 &&
                    bytes[0] == 0x52 &&
                    bytes[1] == 0x49 &&
                    bytes[2] == 0x46 &&
                    bytes[3] == 0x46 &&
                    bytes[8] == 0x57 &&
                    bytes[9] == 0x45 &&
                    bytes[10] == 0x42 &&
                    bytes[11] == 0x50,

                _ => false
            };

            if (!valid)
            {
                throw new Exception(
                    "The uploaded file does not match its selected file type.");
            }
        }

        public async Task <List<DocumentResponseDTO?>> GetPropertyDocuments (Guid propertyId)
        {
            var documents = await _dbContext.Documents
                .AsNoTracking()
                .Where(x =>
                    x.PropertyId == propertyId)
                .OrderByDescending(x => x.UploadedAt)
                .Select(x => new DocumentResponseDTO
                {
                    DocumentID = x.DocumentId,

                    PropertyID = x.PropertyId,

                    TenantID = x.TenantId,

                    LeaseID = x.LeaseId,

                    MaintenanceRequestID =
                        x.MaintenanceRequestId,

                    FileName = x.FileName,

                    ContentType = x.ContentType,

                    FileSize = x.FileSize,

                    Category = x.Category,

                    UploadedAt = x.UploadedAt,

                    UploadedBy = x.UploadedBy,

                    FileUrl =
                (
                    x.ContentType == "image/jpeg" ||
                    x.ContentType == "image/jpg" ||
                    x.ContentType == "image/png" ||
                    x.ContentType == "image/webp"
                )
                ? $"/api/Document/property-image/{x.DocumentId}"
                : $"/api/Document/file/{x.DocumentId}"
                })
                .ToListAsync();

            return documents;
        }

        public async Task<List<DocumentResponseDTO?>> GetLeaseDocuments (Guid leaseId)
        {
            var documents = await _dbContext.Documents
                .AsNoTracking()
                .Where(x =>
                    x.LeaseId == leaseId)
                .OrderByDescending(x => x.UploadedAt)
                .Select(x => new DocumentResponseDTO
                {
                    DocumentID = x.DocumentId,

                    PropertyID = x.PropertyId,

                    TenantID = x.TenantId,

                    LeaseID = x.LeaseId,

                    MaintenanceRequestID =
                        x.MaintenanceRequestId,

                    FileName = x.FileName,

                    ContentType = x.ContentType,

                    FileSize = x.FileSize,

                    Category = x.Category,

                    UploadedAt = x.UploadedAt,

                    UploadedBy = x.UploadedBy,

                    FileUrl =
                        $"/api/Document/file/{x.DocumentId}"
                })
                .ToListAsync();

            return documents;
        }

        public async Task <List<DocumentResponseDTO>> GetMaintenanceDocuments (Guid maintenanceRequestId)
        {
            var documents = await _dbContext.Documents
                .AsNoTracking()
                .Where(x =>
                    x.MaintenanceRequestId ==
                    maintenanceRequestId)
                .OrderByDescending(x => x.UploadedAt)
                .Select(x => new DocumentResponseDTO
                {
                    DocumentID = x.DocumentId,

                    PropertyID = x.PropertyId,

                    TenantID = x.TenantId,

                    LeaseID = x.LeaseId,

                    MaintenanceRequestID =
                        x.MaintenanceRequestId,

                    FileName = x.FileName,

                    ContentType = x.ContentType,

                    FileSize = x.FileSize,

                    Category = x.Category,

                    UploadedAt = x.UploadedAt,

                    UploadedBy = x.UploadedBy,

                    FileUrl =
                        $"/api/Document/file/{x.DocumentId}"
                })
                .ToListAsync();

            return documents;
        }

        public async Task<bool> DeleteDocument (Guid documentId)
        {
            var document = await _dbContext.Documents
               .FirstOrDefaultAsync(x =>
                   x.DocumentId == documentId);

            if (document == null)
            {
                return false;
            }

            _dbContext.Documents.Remove(document);

            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<DocumentFileDTO?> GetPropertyCoverImageFile(Guid propertyId)
        {
            var document = await _dbContext.Documents
                .AsNoTracking()
                .Where(x =>
                    x.PropertyId == propertyId &&
                    (
                        x.ContentType == "image/jpeg" ||
                        x.ContentType == "image/jpg" ||
                        x.ContentType == "image/png" ||
                        x.ContentType == "image/webp"
                    ))
                .OrderBy(x => x.DocumentId)
                .FirstOrDefaultAsync();

            if (document == null)
            {
                return null;
            }

            return new DocumentFileDTO
            {
                DocumentID = document.DocumentId,
                FileName = document.FileName,
                ContentType = document.ContentType,
                FileSize = document.FileSize,
                FileData = document.FileData
            };
        }

        public async Task<DocumentFileDTO?> GetPropertyImageFile(Guid documentId)
        {
            var document = await _dbContext.Documents
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.DocumentId == documentId &&
                    x.PropertyId.HasValue &&
                    (
                        x.ContentType == "image/jpeg" ||
                        x.ContentType == "image/jpg" ||
                        x.ContentType == "image/png" ||
                        x.ContentType == "image/webp"
                    ));

            if (document == null)
            {
                return null;
            }

            return new DocumentFileDTO
            {
                DocumentID = document.DocumentId,
                FileName = document.FileName,
                ContentType = document.ContentType,
                FileSize = document.FileSize,
                FileData = document.FileData
            };
        }

        private static DocumentResponseDTO MapToResponse(Document document)
        {
            return new DocumentResponseDTO
            {
                DocumentID = document.DocumentId,

                PropertyID = document.PropertyId,

                TenantID = document.TenantId,

                LeaseID = document.LeaseId,

                MaintenanceRequestID =
                    document.MaintenanceRequestId,

                FileName = document.FileName,

                ContentType = document.ContentType,

                FileSize = document.FileSize,

                Category = document.Category,

                UploadedAt = document.UploadedAt,

                UploadedBy = document.UploadedBy,

                FileUrl =
                    $"/api/Document/file/{document.DocumentId}"
            };
        }
    }
}
