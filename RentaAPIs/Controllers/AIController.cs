using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentaAPIs.DTOs;
using RentaAPIs.Services;

namespace RentaAPIs.Controllers
{
    [ApiController]
    [Route("api/AI")]
    //[Authorize]
    public class AIController : ControllerBase
    {
        private readonly GeminiLeaseExplanationService _geminiService;
        private readonly ILogger<AIController> _logger;

        public AIController(
            GeminiLeaseExplanationService geminiService,
            ILogger<AIController> logger)
        {
            _geminiService = geminiService;
            _logger = logger;
        }

        [HttpPost("lease-explanation")]
        [ProducesResponseType(
            typeof(LeaseExplanationResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        public async Task<ActionResult<LeaseExplanationResponseDTO>>
            ExplainLease(
                [FromBody] LeaseExplanationRequestDTO request,
                CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.LeaseText))
            {
                return BadRequest(new
                {
                    message = "Please provide lease text to explain."
                });
            }

            try
            {
                string explanation =
                    await _geminiService.ExplainLeaseAsync(
                        request.LeaseText,
                        request.Question,
                        cancellationToken);

                return Ok(new LeaseExplanationResponseDTO
                {
                    Explanation = explanation
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(
                    "Lease explanation could not be completed: {Message}",
                    ex.Message);

                return StatusCode(
                    StatusCodes.Status503ServiceUnavailable,
                    new
                    {
                        message =
                            "The AI explanation service is not ready " +
                            "or returned no explanation. Check the " +
                            "backend configuration and try again."
                    });
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(
                    "Gemini API request failed: {Message}",
                    ex.Message);

                return StatusCode(
                    StatusCodes.Status502BadGateway,
                    new
                    {
                        message =
                            "The AI service could not process the request. " +
                            "Check your model, API access and quota, " +
                            "then try again."
                    });
            }
            catch (OperationCanceledException)
                when (!cancellationToken.IsCancellationRequested)
            {
                return StatusCode(
                    StatusCodes.Status504GatewayTimeout,
                    new
                    {
                        message =
                            "The AI service took too long to respond. " +
                            "Please try again."
                    });
            }
        }
    }
}
