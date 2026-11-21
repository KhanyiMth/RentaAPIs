using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentaAPIs.Services;
using static RentaAPIs.DTOs.RentaMatchDTOs;

namespace RentaAPIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RentaMatchController : ControllerBase
    {
        private readonly RentaMatchService _rentaMatchService;

        public RentaMatchController(RentaMatchService rentaMatchService)
        {
            _rentaMatchService = rentaMatchService;
        }

        [HttpPost("predict")]
        public IActionResult Predict([FromBody] RentaMatchRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new
                    {
                        message = "Prediction data is required."
                    });
                }

                if (request.MonthlyIncome <= 0)
                {
                    return BadRequest(new
                    {
                        message = "Monthly income must be greater than zero."
                    });
                }

                if (request.MonthlyExpenses < 0)
                {
                    return BadRequest(new
                    {
                        message = "Monthly expenses cannot be negative."
                    });
                }

                if (request.HouseholdSize <= 0)
                {
                    return BadRequest(new
                    {
                        message = "Household size must be greater than zero."
                    });
                }

                var expectedRent =
                    _rentaMatchService.PredictExpectedRent(
                        request.MonthlyIncome,
                        request.MonthlyExpenses,
                        request.HouseholdSize
                    );

                return Ok(new
                {
                    expectedRent = expectedRent
                });
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        message = "Unable to generate RentaMatch prediction.",
                        error = ex.Message
                    }
                );
            }
        }
    }

    public class RentaMatchRequest
    {
        public decimal MonthlyIncome { get; set; }

        public decimal MonthlyExpenses { get; set; }

        public int HouseholdSize { get; set; }
    }
}
