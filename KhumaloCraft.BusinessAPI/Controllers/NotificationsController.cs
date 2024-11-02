using KhumaloCraft.Business.Interfaces;
using KhumaloCraft.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KhumaloCraft.BusinessAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationsService _notificationsService;

        public NotificationsController(INotificationsService notificationsService)
        {
            _notificationsService = notificationsService;
        }

        // 1. Add a new notification
        [HttpPost("add")]
        public async Task<IActionResult> AddNotification(string userId, string message)
        {
            await _notificationsService.AddNotificationAsync(userId, message);
            return Ok(new { Message = "Notification added successfully" });
        }

        // 2. Get all notifications for a user
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<NotificationDTO>>> GetNotificationsForUser(string userId)
        {
            var notifications = await _notificationsService.GetNotificationsForUserAsync(userId);
            return Ok(notifications);
        }

        // 3. Get unread notifications for a user
        [HttpGet("user/{userId}/unread")]
        public async Task<ActionResult<IEnumerable<NotificationDTO>>> GetUnreadNotificationsForUser(string userId)
        {
            var unreadNotifications = await _notificationsService.GetUnreadNotificationsForUserAsync(userId);
            return Ok(unreadNotifications);
        }

        // 4. Mark a notification as read
        [HttpPatch("{notificationId}/mark-read")]
        public async Task<IActionResult> MarkNotificationAsRead(int notificationId)
        {
            await _notificationsService.MarkNotificationAsReadAsync(notificationId);
            return Ok(new { Message = "Notification marked as read" });
        }
    }
}
