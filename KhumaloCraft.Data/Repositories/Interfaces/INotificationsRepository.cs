using KhumaloCraft.Data.Entities;

namespace KhumaloCraft.Data.Repositories.Interfaces;

public interface INotificationsRepository
{
  Task AddNotificationAsync(Notification notification);
  Task<IEnumerable<Notification>> GetNotificationsForUserAsync(string userId);
  Task MarkAsReadAsync(int notificationId);
  Task<IEnumerable<Notification>> GetUnreadNotificationsForUserAsync(string userId);
}
