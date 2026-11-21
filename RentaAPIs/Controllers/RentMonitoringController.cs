using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentaAPIs.Services;

namespace RentaAPIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RentMonitoringController : ControllerBase
    {
        private readonly RentMonitoringService _service;
        private readonly IWebHostEnvironment _environment;

        public RentMonitoringController(
            RentMonitoringService service,
            IWebHostEnvironment environment)
        {
            _service = service;
            _environment = environment;
        }

        [HttpPost("run")]
        public async Task<IActionResult> Run()
        {
            // Temporary development-only endpoint.
            if (!_environment.IsDevelopment())
            {
                return NotFound();
            }

            // Do not expose system-wide monitoring to
            // ordinary authenticated users.
            return StatusCode(403, new
            {
                message =
                    "Manual monitoring is disabled. " +
                    "Use the background monitoring service."
            });
        }
    }
}


