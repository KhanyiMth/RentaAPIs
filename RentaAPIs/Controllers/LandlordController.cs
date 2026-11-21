using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RentaAPIs.dtos;
using RentaAPIs.DTOs;
using RentaAPIs.Services;
using System.Globalization;
using System.Security.Claims;

namespace RentaAPIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LandlordController : ControllerBase
    {
        private readonly LandlordService _landlordService;

        public LandlordController(LandlordService landlordService)
        {
            _landlordService = landlordService;
        }

        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetLandlordById (Guid id)
        {
            var landlord = await _landlordService.GetLandlordById(id);

            return Ok(landlord);
        }

        [Authorize]
        [HttpGet("get-me")]
        public async Task<IActionResult> GetMe()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var landlord = await _landlordService.GetMe(userId);

                return Ok(landlord);
            }
            catch (Exception ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateLandlord (RegisterLandlordDTO rlDTO)
        {
            await _landlordService.CreateLandlord(rlDTO);

            return Ok("Landlord registered successfully");
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateLandlord (Guid id, UpdateLandlordDTO ulDTO)
        {
            var updatedLandlord = await _landlordService.UpdateLandlord(id, ulDTO);

            if (!updatedLandlord)
                return NotFound();

            return NoContent();
        }

    }
}
