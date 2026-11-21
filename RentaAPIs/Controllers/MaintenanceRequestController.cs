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
    public class MaintenanceRequestController : ControllerBase
    {
        public MaintenanceRequestService _mrService;

        public MaintenanceRequestController (MaintenanceRequestService mrService)
        {
            _mrService = mrService;
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllMaintenanceRequests ()
        {
            return Ok(await _mrService.GetAllMaintenanceRequests());
        }

        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetMaintenanceRequestById (Guid id)
        {
            var request = await _mrService.GetMaintenanceRequestById(id);

            if (request == null)
                return NotFound();

            return Ok(request);
        }

        [HttpGet("landlordRequests")]
        [Authorize]
        public async Task<IActionResult> GetLandlordMaintenanceRequests()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if(string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new
                    {
                        message = "User ID not found in token"
                    });
                }

                var requests = await _mrService.GetLandlordMaintenanceRequests(userId);

                return Ok(requests);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message,
                });
            }
        }


        [HttpPost("create")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<IActionResult> CreateMaintenanceRequest(CreateMaintenanceRequestDTO cmrequest)
        {
            try
            {
                var userId = User.FindFirstValue(
                    ClaimTypes.NameIdentifier
                );

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new
                    {
                        message = "User is not authenticated"
                    });
                }

                var result = await _mrService.CreateMaintenanceRequest(
                    cmrequest,
                    userId
                );

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                // Log the full exception on the server.
                // Avoid exposing internal database details to clients.
                return StatusCode(500, new
                {
                    message = "Unable to create maintenance request."
                });
            }
        }


        [HttpGet("{id:guid}/image")]
        public async Task<IActionResult> GetMaintenanceImage(Guid id)
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new
                {
                    message = "User is not authenticated."
                });
            }

            try
            {
                var imagePath = await _mrService
                    .GetAuthorisedMaintenanceImagePath(id, userId);

                if (imagePath == null)
                {
                    return NotFound(new
                    {
                        message = "Maintenance image not found."
                    });
                }

                Response.Headers.CacheControl = "no-store";

                return PhysicalFile(imagePath, "image/jpeg");
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }


        [HttpGet("getmaintenancerequests")]
        public async Task<IActionResult> GetTenantMaintenanceRequest()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if(string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new
                    {
                        message = "User ID not found in token"
                    });
                }

                var requests = await _mrService.GetTenantMaintenanceRequests(userId);

                return Ok(requests);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPut("update/{id:guid}")]
        public async Task<IActionResult> UpdateMaintenanceRequest(Guid id, [FromBody] UpdateMaintenanceRequestDTO umrequest)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            try
            {
                var updated = await _mrService.UpdateMaintenanceRequest(
                    id,
                    umrequest,
                    userId);

                if (!updated)
                    return NotFound(new
                    {
                        message = "Maintenance request not found."
                    });

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteMaintenanceRequest (Guid id)
        {
            var deleteRequest = await _mrService.DeleteMaintenanceRequest(id);

            if (!deleteRequest)
                return NotFound();

            return NoContent();
        }

    }
}
