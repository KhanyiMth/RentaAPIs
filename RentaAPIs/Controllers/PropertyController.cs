using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RentaAPIs.cdDTOs;
using RentaAPIs.DTOs;
using RentaAPIs.Models;
using RentaAPIs.Services;
using System.Security.Claims;

namespace RentaAPIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PropertyController : ControllerBase
    {
        private readonly PropertyService _propertyService;

        public PropertyController(PropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllProperties ()
        {
            var properties = await _propertyService.GetAllProperties();

            return Ok(properties);
        }

        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetPropertyById (Guid id)
        {
            var property = await _propertyService.GetPropertyById(id);

            if (property == null)
            {
                return NotFound("Property not found.");
            }

            return Ok(property);
        }

        [Authorize]
        [HttpGet("my-properties")]
        public async Task<IActionResult> GetMyProperties()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var properties = await _propertyService.GetMyProperties(userId);

                return Ok(properties);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        //[Authorize(Roles = "Landlord")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateProperty (CreatePropertyDTO createDTO)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _propertyService.CreateProperty(userId, createDTO);

            return Ok(result);
        }

        //[Authorize(Roles = "Landlord")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateProperty (Guid id, UpdatePropertyDTO updateDTO)
        {
            var updatedProperty = await _propertyService.UpdateProperty(id, updateDTO);

            if (!updatedProperty)
                return NotFound();

            return NoContent();
        }

        //[Authorize(Roles = "Landlord")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteProperty (Guid id)
        {
            var deleteProperty = await _propertyService.DeleteProperty(id);

            if (!deleteProperty)
                return NotFound();

            return NoContent();
        }

    }
}
