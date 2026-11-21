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
    public class PaymentController : ControllerBase
    {
        public PaymentService _pService;

        public PaymentController(PaymentService pService)
        {
            _pService = pService;
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllPayments ()
        {
            return Ok(await _pService.GetAllPayments());
        }

        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetPaymentById (Guid id)
        {
            var payment = await _pService.GetPaymentById(id);

            if (payment == null)
                return NotFound();

            return Ok(payment);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment ([FromBody] CreatePaymentDTO cpDTO)
        {
            try
            {
                var payment =
                    await _pService.CreatePayment(cpDTO);

                return CreatedAtAction(
                    nameof(GetPaymentById),
                    new
                    {
                        id = payment.PaymentID
                    },
                    payment
                );
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new
                    {
                        message = ex.Message
                    }
                );
            }
        }

        [HttpPost("mock-pay")]
        public async Task <IActionResult> MockPay([FromBody] MockPaymentDTO mpDTO)
        {
            try
            {
                if (mpDTO == null)
                {
                    return BadRequest(new
                    {
                        message = "Payment information is required"
                    });
                }

                if (mpDTO.LeaseId == Guid.Empty)
                {
                    return BadRequest(new
                    {
                        message = "A valid lease is required."
                    });
                }

                if (mpDTO.Amount <= 0)
                {
                    return BadRequest(new
                    {
                        message = "Payment amount must be greater than zero."
                    });
                }


                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");

                if (string.IsNullOrWhiteSpace(userId))
                {
                    return Unauthorized(new
                    {
                        message = "Unable to identify the logged-in user"
                    });
                }

                var payment = await _pService.MockPay(userId, mpDTO.LeaseId, mpDTO.Amount, mpDTO.PaymentMethod);

                if (payment == null)
                {
                    return BadRequest(new
                    {
                        message = "The payment could not be processed"
                    });
                }

                return Ok(payment);

            }
            catch(Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPost("mock-success/{id}")]
        public async Task<IActionResult> MockPaymentSuccess(
            Guid id)
        {
            var payment =
                await _pService.MockPaymentSuccess(id);

            if (payment == null)
            {
                return NotFound(
                    new
                    {
                        message = "Payment not found."
                    }
                );
            }

            return Ok(payment);
        }

        [HttpPost("mock-failed/{id}")]
        public async Task<IActionResult> MockPaymentFailed(
            Guid id)
        {
            var payment =
                await _pService.MockPaymentFailed(id);

            if (payment == null)
            {
                return NotFound(
                    new
                    {
                        message = "Payment not found."
                    }
                );
            }

            return Ok(payment);
        }

        [HttpGet("tenant/{tenantId}")]
        public async Task<IActionResult> GetTenantPayments(
            Guid tenantId)
        {
            var payments =
                await _pService.GetTenantPayments(tenantId);

            return Ok(payments);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdatePayment (Guid id, UpdatePaymentDTO upDTO)
        {
            var updatedPayment = await _pService.UpdatePayment(id, upDTO);

            if (!updatedPayment)
                return NotFound();

            return NoContent();
        }

        [HttpGet("tenant/{tenantId}/next")]
        public async Task<IActionResult> GetNextTenantPayment(
            Guid tenantId)
        {
            var payment =
                await _pService.GetNextTenantPayment(tenantId);

            if (payment == null)
            {
                return NotFound(
                    new
                    {
                        message =
                            "No outstanding payment found."
                    }
                );
            }

            return Ok(payment);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeletePayment (Guid id)
        {
            var deletePayment = await _pService.DeletePayment(id);

            if (!deletePayment)
                return NotFound();

            return NoContent();
        }

    }
}
