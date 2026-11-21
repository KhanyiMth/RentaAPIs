using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentaAPIs.DTOs;
using RentaAPIs.Services;

namespace RentaAPIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        public NotificationService _nService;

        public NotificationController (NotificationService nService)
        {
            _nService = nService;
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllNotifications ()
        {
            return Ok(await _nService.GetAllNotifications());
        }

        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetNotificationById (Guid id)
        {
            var notification = await _nService.GetNotificationById(id);

            if (notification == null)
                return NotFound();

            return Ok(notification);
        }

        [HttpGet("getUserNotifications/{userId}")]
        public async Task<IActionResult> GetUserNotifications (Guid userId)
        {
            return Ok(await _nService.GetUserNotifications(userId));
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateNotification(CreateNotificationDTO cnDTO)
        {
            var notification = await _nService.CreateNotification(cnDTO);

            return CreatedAtAction(nameof(GetNotificationById), new { id = notification.NotificationID }, notification);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var updateNotification = await _nService.MarkAsRead(id);

            if (!updateNotification)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteNotification(Guid id)
        {
            var deleteNotification = await _nService.DeleteNotification(id);

            if (!deleteNotification)
                return NotFound();

            return NoContent();
        }

    }
}
