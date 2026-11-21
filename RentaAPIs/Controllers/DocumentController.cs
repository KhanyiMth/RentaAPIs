using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentaAPIs.DTOs;
using RentaAPIs.Services;
using System.Security.Claims;

namespace RentaAPIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DocumentController : ControllerBase
    {
        public DocumentService _dService;

        public DocumentController(DocumentService dService)
        {
            _dService = dService;
        }

        [HttpPost("upload")]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task <IActionResult> Upload([FromForm] CreateDocumentDTO cdDto)
        {
            try
            {
                var document =
                    await _dService.UploadDocument(cdDto);

                return Ok(document);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPost("upload-base64")]
        [RequestSizeLimit(15 * 1024 * 1024)]
        public async Task<IActionResult> UploadBase64([FromBody] Base64DocumentUploadDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new
                    {
                        message = "Document upload data is required."
                    });
                }

                Guid? uploadedBy = null;

                var userIdClaim =
                    User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!string.IsNullOrWhiteSpace(userIdClaim))
                {
                    if (Guid.TryParse(userIdClaim, out Guid parsedUserId))
                    {
                        uploadedBy = parsedUserId;
                    }
                }

                var document =
                    await _dService.UploadBase64Document(
                        dto,
                        uploadedBy);

                return Ok(document);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DOCUMENT UPLOAD ERROR: {ex}");

                return BadRequest(new
                {
                    message = ex.Message,
                    innerException = ex.InnerException?.Message,
                    innerInnerException = ex.InnerException?.InnerException?.Message
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById (Guid id)
        {
            var document =
                await _dService.GetDocumentById(id);

            if (document == null)
            {
                return NotFound(new
                {
                    message = "Document not found."
                });
            }

            return Ok(document);
        }


        [HttpGet("file/{id}")]
        public async Task <IActionResult?> GetFile (Guid id)
        {
            var document =
                await _dService.GetDocumentFile(id);

            if (document == null)
            {
                return NotFound(new
                {
                    message = "Document not found."
                });
            }

            return File(
                document.FileData,
                document.ContentType,
                document.FileName
            );
        }

        [AllowAnonymous]
        [HttpGet("property-image/{id}")]
        public async Task<IActionResult> GetPropertyImage(Guid id)
        {
            var document =
                await _dService.GetPropertyImageFile(id);

            if (document == null)
            {
                return NotFound(new
                {
                    message = "Property image not found."
                });
            }

            return File(
                document.FileData,
                document.ContentType
            );
        }

        [HttpGet("tenant/{tenantId}")]
        public async Task<IActionResult> GetTenantDocuments (Guid tenantId)
        {
            var documents =
                await _dService
                    .GetTenantDocuments(tenantId);

            return Ok(documents);
        }

        [HttpGet("property/{propertyId}")]
        public async Task<IActionResult> GetPropertyDocuments (Guid propertyId)
        {
            var documents =
                await _dService
                    .GetPropertyDocuments(propertyId);

            return Ok(documents);
        }

        [HttpGet("lease/{leaseId}")]
        public async Task<IActionResult> GetLeaseDocuments (Guid leaseId)
        {
            var documents =
                await _dService
                    .GetLeaseDocuments(leaseId);

            return Ok(documents);
        }

        [HttpGet("maintenance/{maintenanceRequestId}")]
        public async Task<IActionResult> GetMaintenanceDocuments(Guid maintenanceRequestId)
        {
            var documents =
                await _dService
                    .GetMaintenanceDocuments(
                        maintenanceRequestId);

            return Ok(documents);
        }

        [AllowAnonymous]
        [HttpGet("property-cover/{propertyId}")]
        public async Task<IActionResult> GetPropertyCover(Guid propertyId)
        {
            var document = await _dService.GetPropertyCoverImageFile(propertyId);

            if (document == null)
            {
                return NotFound(new
                {
                    message = "Property cover image not found."
                });
            }

            return File(
                document.FileData,
                document.ContentType
            );
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete (Guid id)
        {
            var deleted =
                await _dService
                    .DeleteDocument(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = "Document not found."
                });
            }

            return Ok(new
            {
                message = "Document deleted successfully."
            });
        }
    }
}
