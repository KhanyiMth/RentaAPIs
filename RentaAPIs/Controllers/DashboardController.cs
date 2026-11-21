using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentaAPIs.Services;
using System.Security.Claims;

namespace RentaAPIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly DashboardService _dService;

        public DashboardController (DashboardService dService)
        {
            _dService = dService;
        }

        [HttpGet("getDashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var userIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

            if (
                string.IsNullOrWhiteSpace(userIdValue) ||
                !Guid.TryParse(userIdValue, out var userId)
            )
            {
                return Unauthorized(new
                {
                    message = "Invalid or missing user ID."
                });
            }

            try
            {
                var dashboard = await _dService.GetDashboard(userId);

                return Ok(dashboard);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
        }

    }
}
