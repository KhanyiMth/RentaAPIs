using Microsoft.EntityFrameworkCore;
using RentaAPIs.dtos;
using RentaAPIs.DTOs;
using RentaAPIs.Models;

namespace RentaAPIs.Services
{
    public class NotificationService
    {
        public AppDbContext _dbContext;

        public NotificationService (AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<NotificationResponseDTO>> GetAllNotifications ()
        {
            return await _dbContext.Notifications
                .Select(n => new NotificationResponseDTO
                {
                    NotificationID = n.NotificationID,
                    UserID = n.UserID,
                    Title = n.Title,
                    Message = n.Message,
                    Type = n.Type,
                    IsRead = n.IsRead,
                    ActionUrl = n.ActionUrl,
                    CreatedAt = n.CreatedAt,
                    ReadAt = n.ReadAt

                }).ToListAsync();
        }

        public async Task<NotificationResponseDTO> GetNotificationById (Guid id)
        {
            return await _dbContext.Notifications
                .Where(n => n.NotificationID == id)
                .Select(n => new NotificationResponseDTO
                {
                    NotificationID = n.NotificationID,
                    UserID = n.UserID,
                    Title = n.Title,
                    Message = n.Message,
                    Type = n.Type,
                    IsRead = n.IsRead,
                    ActionUrl = n.ActionUrl,
                    CreatedAt = n.CreatedAt,
                    ReadAt = n.ReadAt

                }).FirstOrDefaultAsync();
        }

        public async Task<List<NotificationResponseDTO>> GetUserNotifications (Guid userId)
        {
            return await _dbContext.Notifications
                .Where(n => n.UserID == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationResponseDTO
                {
                    NotificationID = n.NotificationID,
                    UserID = n.UserID,
                    Title = n.Title,
                    Message = n.Message,
                    Type = n.Type,
                    IsRead = n.IsRead,
                    ActionUrl = n.ActionUrl,
                    CreatedAt = n.CreatedAt,
                    ReadAt = n.ReadAt

                }).ToListAsync();
        }

        public async Task<NotificationResponseDTO> CreateNotification (CreateNotificationDTO cnDTO)
        {
            var notification = new Notification
            {
                UserID = cnDTO.UserID,
                Title = cnDTO.Title,
                Message = cnDTO.Message,
                Type = cnDTO.Type,
                ActionUrl = cnDTO.ActionUrl,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Notifications.Add(notification);
            await _dbContext.SaveChangesAsync();

            return await GetNotificationById(notification.NotificationID) ?? throw new Exception("Unable to retrieve notification");
        }

        public async Task<bool> MarkAsRead (Guid id)
        {
            var notification = await _dbContext.Notifications.FindAsync(id);

            if (notification is null)
                return false;

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteNotification (Guid id)
        {
            var notification = await _dbContext.Notifications.FindAsync(id);

            if (notification == null)
                return false;

            _dbContext.Notifications.Remove(notification);
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
