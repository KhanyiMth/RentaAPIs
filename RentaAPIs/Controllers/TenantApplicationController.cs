using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentaAPIs.dtos;
using RentaAPIs.DTOs;
using RentaAPIs.Services;
using System.Security.Claims;

namespace RentaAPIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TenantApplicationController : ControllerBase
    {
        private readonly TenantApplicationService _tenantApplicationService;

        public TenantApplicationController(TenantApplicationService tenantApplicationService)
        {
            _tenantApplicationService = tenantApplicationService;
        }

        [HttpGet("preview/{advertisementId:guid}")]
        public async Task <IActionResult> GetPreview(Guid advertisementId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if(string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new
                    {
                        message = "User identity not found (Get Preview)"
                    });
                }

                var preview = await _tenantApplicationService.GetApplicationPreview(advertisementId, userId);

                return Ok(preview);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Unable to create application preview",
                    error = ex.Message,
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateApplication([FromBody] CreateTenantApplicationdto ctadto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if(string.IsNullOrWhiteSpace(userId))
                {
                    return Unauthorized(new
                    {
                        message = "User identity was not found (Create Application)"
                    });
                }

                if(ctadto == null || ctadto.AdvertisementId == Guid.Empty)
                {
                    return BadRequest(new
                    {
                        message = "AdvertisementId is required"
                    });
                }

                var application = await _tenantApplicationService.CreateApplication(ctadto, userId);

                return CreatedAtAction(nameof(GetApplication), new
                {
                    applicationId = application.TenantApplicationId,
                }, application);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Unable to submit application",
                    error = ex.Message,
                });
            }
        }

        [HttpGet("{applicationId:guid}")]
        public async Task<IActionResult> GetApplication (Guid applicationId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            var application = await _tenantApplicationService.GetApplication(applicationId, userId);

            if(application == null)
            {
                return NotFound(new
                {
                    message = "Application not found"
                });
            }

            return Ok(application);
        }

        [HttpGet("received")]
        public async Task<IActionResult> GetReceivedApplications()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if(string.IsNullOrWhiteSpace(userId))
                {
                    return Unauthorized(new
                    {
                        message = "User identity was not found (Get Received Application)"
                    });
                }

                var applications = await _tenantApplicationService.GetReceivedApplications(userId);

                return Ok(applications);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Unable to load received applications",
                    error = ex.Message,
                });
            }
        }


        [HttpGet("my-applications")]
        public async Task<IActionResult> GetMyApplications()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue("sub")
                    ?? User.FindFirstValue("userId")
                    ?? User.FindFirstValue("UserId")
                    ?? User.FindFirstValue("namdid");

                if(string.IsNullOrWhiteSpace(userId))
                {
                    return Unauthorized(new
                    {
                        message = "User identity was not found"
                    });
                }

                var applications = await _tenantApplicationService.GetMyApplications(userId);

                return Ok(applications);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GET MY APPLICATIONS ERROR: {ex}");

                return StatusCode(500, new
                {
                    message = "Unable to load your applications",
                    error = ex.Message,
                });
            }

        }


        [HttpPatch("{applicationId:guid}/status")]
        public async Task<IActionResult> UpdateApplicationStatus(Guid applicationId, [FromBody] UpdateTenantApplicationStatusDTO dto)
        {
            try
            {
                var userId =
                    User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue("sub")
                    ?? User.FindFirstValue("userId")
                    ?? User.FindFirstValue("UserId")
                    ?? User.FindFirstValue("nameid");

                if (string.IsNullOrWhiteSpace(userId))
                {
                    return Unauthorized(new
                    {
                        message = "User identity was not found."
                    });
                }

                if (dto == null || string.IsNullOrWhiteSpace(dto.Status))
                {
                    return BadRequest(new
                    {
                        message = "Status is required."
                    });
                }

                var application =
                    await _tenantApplicationService.UpdateApplicationStatus(
                        applicationId,
                        userId,
                        dto.Status
                    );

                return Ok(application);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"UPDATE APPLICATION STATUS ERROR: {ex}"
                );

                return StatusCode(500, new
                {
                    message = "Unable to update application status.",
                    error = ex.Message
                });
            }
        }
    }
}
