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
    public class LeaseController : ControllerBase
    {
        private readonly LeaseService _leaseService;

        public LeaseController (LeaseService leaseService)
        {
            _leaseService = leaseService;
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllLeases ()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var leases = await _leaseService.GetAllLeases();

            return Ok(leases);
        }


        [HttpGet("create-data/{applicationId}")]
        public async Task<IActionResult> GetLeaseCreationData(Guid applicationId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            try
            {
                var data = await _leaseService.GetLeaseCreationData(applicationId, userId);

                if (data == null)
                    return NotFound(new
                    {
                        message = "Accepted application could not be found"
                    });

                return Ok(data);
            }
            catch(UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message,
                });
            }
        }


        [HttpPost("sign")]
        public async Task<IActionResult> SignLease(Guid leaseId, [FromBody] SignLeaseRequestDTO slrDTO)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            if (slrDTO == null || slrDTO.LeaseId == Guid.Empty)
            {
                return BadRequest(new
                {
                    message = "A valid lease ID is required"
                });
            }

            if(string.IsNullOrWhiteSpace(slrDTO.Signature))
            {
                return BadRequest(new
                {
                    message = "A signature is required"
                });
            }

            try
            {
                var signLeaseDTO = new SignLeaseDTO
                {
                    Signature = slrDTO.Signature,
                };

                var lease = await _leaseService.SignLease(slrDTO.LeaseId, signLeaseDTO, userId);

                return Ok(new
                {
                    message = "Lease signed successfully and returned to the landlord",
                    lease
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message,
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("========== SIGN LEASE ERROR ==========");
                Console.WriteLine(ex.ToString());

                if (ex.InnerException != null)
                {
                    Console.WriteLine("INNER EXCEPTION:");
                    Console.WriteLine(ex.InnerException.ToString());
                }

                Console.WriteLine("======================================");

                return StatusCode(500, new
                {
                    message = "Unable to sign lease",
                    error = ex.Message,

                    // TEMPORARY: Only use this while debugging locally.
                    innerError = ex.InnerException?.Message
                });
            }
        }

        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetLeaseById (Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            try
            {
                var lease = await _leaseService.GetLeaseById(id, userId);

                if (lease == null)
                    return NotFound();

                return Ok(lease);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }

        }


        [HttpGet("by-application/{applicationId:guid}")]
        public async Task <IActionResult> GetLeaseByApplication(Guid applicationId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            try
            {
                var lease = await _leaseService.GetLeaseByApplication(applicationId, userId);

                if(lease == null)
                {
                    return NotFound(new
                    {
                        message = "The lease for this accepted application could not found"
                    });
                }

                return Ok(lease);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new 
                {
                    message = ex.Message,
                });
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateLease ([FromBody] CreateLeaseDTO clDTO)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            try
            {
                var newLease = await _leaseService.CreateLease(clDTO, userId);

                return CreatedAtAction(nameof(GetLeaseById), new
                {
                    id = newLease.LeaseId
                }, newLease);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message,
                });
            }
        }





        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteLease (Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var deleted = await _leaseService.DeleteLease(id, userId);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
