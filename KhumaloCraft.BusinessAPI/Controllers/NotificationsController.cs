using KhumaloCraft.Business.Interfaces;
using KhumaloCraft.Shared.DTOs;
using KhumaloCraft.Shared.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace KhumaloCraft.BusinessAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationsService _notificationsService;
        private readonly IHubContext<NotificationHub> _hubContext;


        public NotificationsController(INotificationsService notificationsService, IHubContext<NotificationHub> hubContext)
        {
            _notificationsService = notificationsService;
            _hubContext = hubContext;
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

        [HttpPost("notification-order-status")]
        public async Task<IActionResult> SendOrderStatusNotification([FromBody] NotificationRequest message)
        {
            if (!string.IsNullOrEmpty(message.UserId))
            {
                // Format the message to be sent
                var formattedMessage = $"Order: {message.OrderId} updated to {message.Status}";

                // Send the formatted message to the specific user group
                await _hubContext.Clients.Group(message.UserId).SendAsync("ReceiveNotification", formattedMessage);

                return Ok("Notification sent to specific user.");
            }

            return BadRequest("User not found for the specified order.");
        }

        [HttpPost("notification-product-update")]
        public async Task<IActionResult> SendProductNotification([FromBody] ProductNotificationsRequest message)
        {
            // Format the message to be sent
            var formattedMessage = $"{message.ProductName} - {message.Message}";

            // Send the formatted message to all connected clients
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", formattedMessage);

            return Ok("Notification sent to all clients.");
        }
    }
}
