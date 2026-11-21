using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentaAPIs.Services;

namespace RentaAPIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RentaVisionController : ControllerBase
    {
        private readonly RentaVisionService _visionService;
        private readonly ILogger<RentaVisionController> _logger;

        public RentaVisionController(
            RentaVisionService visionService,
            ILogger<RentaVisionController> logger)
        {
            _visionService = visionService;
            _logger = logger;
        }

        [HttpPost("predict")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<IActionResult> Predict(IFormFile file)
        {
            try
            {
                var prediction =
                    await _visionService.PredictAsync(file);

                return Ok(prediction);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (SixLabors.ImageSharp.UnknownImageFormatException)
            {
                return BadRequest(new
                {
                    message = "The uploaded file is not a supported image."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "RentaVision prediction failed."
                );

                return StatusCode(500, new
                {
                    message = "Unable to analyse the maintenance image."
                });
            }
        }
        }
}
