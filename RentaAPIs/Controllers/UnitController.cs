using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentaAPIs.DTOs;
using RentaAPIs.Services;

namespace RentaAPIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UnitController : ControllerBase
    {
        private readonly UnitService _unitService;

        public UnitController(UnitService unitService)
        {
            _unitService = unitService;
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllUnits ()
        {
            var units = await _unitService.GetAllUnits();

            return Ok(units);
        }

        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetUnitById (Guid id)
        {
            var unit = await _unitService.GetUnitById(id);

            return Ok(unit);
        }

        [HttpGet("property/{propertyId:guid}")]
        public async Task<IActionResult> GetUnitsByPropertyId (Guid propertyId)
        {
            try
            {
                var units = await _unitService.GetUnitsByPropertyID(propertyId);

                return Ok(units);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while retrieving the units",
                    error = ex.Message,
                });
            }

        }

        [Authorize(Roles = "Landlord")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateUnit (CreateUnitDTO cuDTO)
        {
            var newUnit = await _unitService.CreateUnit(cuDTO);

            return CreatedAtAction(nameof(GetUnitById), new { id = newUnit.UnitId }, newUnit);
        }

        [Authorize(Roles = "Landlord")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUnit (Guid id, UpdateUnitDTO uuDTO)
        {
            var updatedUnit = await _unitService.UpdateUnit(id, uuDTO);

            if (!updatedUnit)
                return NotFound();

            return NoContent();
        }

        [Authorize(Roles = "Landlord")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUnit(Guid id)
        {
            var deleteUnit = await _unitService.DeleteUnit(id);

            if (!deleteUnit)
                return NotFound();

            return NoContent();
        }

    }
}
