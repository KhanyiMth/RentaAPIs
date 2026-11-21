using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentaAPIs.DTOs;
using RentaAPIs.Services;
using System.Security.Claims;

namespace RentaAPIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TenantController : ControllerBase
    {
        private readonly TenantService _tenantService;

        public TenantController(TenantService tenantService)
        {
            _tenantService = tenantService;
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllTenants ()
        {
            var tenants = await _tenantService.GetAllTenants();

            return Ok(tenants);
        }

        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetTenantById (Guid id)
        {
            var tenant = await _tenantService.GetTenantById(id);

            return Ok(tenant);
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetTenantDashboard ()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new
                {
                    message = "User ID not found in token"
                });
            }

            try
            {
                var dashboard = await _tenantService.GetTenantDashboard(userId);

                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateTenant (CreateTenantDTO ctDTO)
        {
            await _tenantService.CreateTenant(ctDTO);

            return Ok("Tenant successfully registered");
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateTenant (Guid id, UpdateTenantDTO utDTO)
        {
            var updatedTenant = await _tenantService.UpdateTenant(id, utDTO);

            if (!updatedTenant)
                return NotFound();

            return NoContent();
        }

        [Authorize]
        [HttpGet("get-me")]
        public async Task<IActionResult> GetMyTenantProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new
                {
                    message = "User ID not found in token"
                });
            }

            try
            {
                var tenant = await _tenantService.GetTenantByUserId(userId);

                if (tenant == null)
                {
                    return NotFound(new
                    {
                        message = "Tenant profile not found"
                    });
                }

                return Ok(tenant);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteTenant (Guid id)
        {
            var deleteTenant = await _tenantService.DeleteTenant(id);

            if (!deleteTenant)
                return NotFound();

            return NoContent();
        }


    }
}
