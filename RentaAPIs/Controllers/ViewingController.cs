using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentaAPIs.dtos;
using RentaAPIs.DTOs;
using RentaAPIs.Services;
using System.Security.Claims;

namespace RentaAPIs.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ViewingController : ControllerBase
    {
        private readonly ViewingService _viewingService;
        private readonly AppDbContext _dbContext;

        public ViewingController(ViewingService viewingService, AppDbContext dbContext)
        {
            _viewingService = viewingService;
            _dbContext = dbContext;
        }

        [HttpPost("availability")]
        public async Task <IActionResult> CreateAvailability([FromBody] CreateViewingAvailabilityDTO cvDTO)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userId == null)
                {
                    return Unauthorized(new
                    {
                        message = "User ID not found in token"
                    });
                }

                if(!Guid.TryParse(userId, out Guid userGuid))
                {
                    return BadRequest(new
                    {
                        message = "Invalid User ID"
                    });
                }

                var availability = await _viewingService.CreateAvailability(cvDTO, userGuid);

                return Ok(new
                {
                    success = true,
                    message = "Viewing availability created successfully",
                    availabilityId = availability.ViewingAvailabilityId
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while creating viewing availability.",
                    error = ex.Message
                });
            }
        }

        [HttpPost("generate-schedule")]
        public async Task<IActionResult> GenerateViewingSchedule([FromBody] CreateViewingScheduleDTO cvsDTO)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if(userId == null)
                {
                    return Unauthorized(new
                    {
                        message = "User ID not found in token"
                    });
                }

                if(!Guid.TryParse(userId, out Guid userGuid))
                {
                    return BadRequest(new
                    {
                        message = "Invalid User ID"
                    });
                }

                var result = await _viewingService.GenerateViewingSchedule(cvsDTO, userGuid);

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = ex.Message,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occured while generating the viewing schedule",
                    error = ex.Message,
                });
            }
        }

        [AllowAnonymous]
        [HttpGet("property/{propertyId}/availability")]
        public async Task<IActionResult> GetAvailableViewings(Guid propertyId)
        {
            try
            {
                var result = await _viewingService.GetAvailableViewings(propertyId);

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPost("book")]
        public async Task<IActionResult> BookViewing([FromBody] CreateViewingBookingDTO cbDTO)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userId == null)
                    return Unauthorized();

                if(!Guid.TryParse(userId, out Guid tenantId))
                {
                    return BadRequest(new
                    {
                        message = "Invalid User ID"
                    });
                }

                var booking = await _viewingService.BookViewing(cbDTO, tenantId);

                return Ok(new
                {
                    success = true,
                    message = "Viewing booked successfully",
                    bookingId = booking.ViewingBookingId
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpGet("my-bookings")]
        public async Task <IActionResult> GetMyBookings()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if(string.IsNullOrWhiteSpace(userId))
                {
                    return Unauthorized(new
                    {
                        message = "The authenticated user ID could not be found"
                    });
                }

                var tenant = await _dbContext.Tenants.AsNoTracking().FirstOrDefaultAsync(t => t.UserId == userId);

                if(tenant == null)
                {
                    return NotFound(new
                    {
                        message = "The authenticated user is not linked to a tenant profile."
                    });
                }

                var tenantId = tenant.TenantId;

                var bookings = await _viewingService.GetTenantBookings(tenantId);

                return Ok(bookings);
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An error occured while loading your viewing bookins."
                });
            }
        }

        [HttpPatch("{bookingId}/cancel")]
        public async Task<IActionResult> CancelViewing (Guid bookingId)
        {
            try
            {
                if(bookingId == Guid.Empty)
                {
                    return BadRequest(new
                    {
                        message = "A valid booking ID is required"
                    });
                }
                
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrWhiteSpace(userId))
                {
                    return Unauthorized(new
                    {
                        message = "The authenticated user ID could not be found"
                    });
                }

                var tenant = await _dbContext.Tenants.FirstOrDefaultAsync(t => t.UserId == userId);

                if (tenant == null)
                {
                    return NotFound(new
                    {
                        message = "The authenticated user is not linked to a tenant profile."
                    });
                }

                var tenantId = tenant.TenantId;

                await _viewingService.CancelViewing(bookingId, tenantId);

                return Ok(new
                {
                    success = true,
                    message = "Viewing cancelled successfully"
                });
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}
