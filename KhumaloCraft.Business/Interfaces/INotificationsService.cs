using KhumaloCraft.Shared.DTOs;

namespace KhumaloCraft.Business.Interfaces;

public interface INotificationsService
{
  Task AddNotificationAsync(string userId, string message);
  Task AddNotificationToAllUsersAsync(string message);
  Task<IEnumerable<NotificationDTO>> GetNotificationsForUserAsync(string userId);
  Task MarkNotificationAsReadAsync(int notificationId);
  Task<IEnumerable<NotificationDTO>> GetUnreadNotificationsForUserAsync(string userId);
}
