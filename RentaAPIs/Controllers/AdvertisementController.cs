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
    public class AdvertisementController : ControllerBase
    {
        private readonly AdvertisementService _adService;

        public AdvertisementController(AdvertisementService advertisementService)
        {
            _adService = advertisementService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAdvertisement([FromBody] CreateAdvertisementDTO caDTO)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var advertisement = await _adService.CreateAdvertisement(userId, caDTO);

                return CreatedAtAction(
                    nameof(GetAdvertisement),
                    new
                    {
                        id = advertisement.AdvertisementId

                    }, advertisement);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message,
                });
            }
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAdvertisement (Guid id)
        {
            try
            {
                var advertisement = await _adService.GetAdvertisement(id);

                return Ok(advertisement);
            }
            catch (Exception ex)
            {
                return NotFound(new
                {
                    message = ex.Message,
                });
            }
        }

        [HttpGet("allAdvertisements")]
        public async Task<IActionResult> GetAllAdvertised()
        {
            try
            {
                var advertisements = await _adService.GetAllAdvertisements();

                return Ok(advertisements);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Failed to load advertised properties",
                    detail = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        [HttpDelete("delete/{id:guid}")]
        public async Task<IActionResult> DeleteAdvertisement(Guid id)
        {
            var userId =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                var deleted =await _adService.DeleteAdvertisement(id, userId);

                if (!deleted)
                {
                    return NotFound(new
                    {
                        message = "Advertisement not found."
                    });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

    }
}
