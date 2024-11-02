using KhumaloCraft.Business.Interfaces;
using KhumaloCraft.Data.Entities;
using KhumaloCraft.Data.Repositories.Interfaces;
using KhumaloCraft.Shared.DTOs;

namespace KhumaloCraft.Business.Services;

public class NotificationsService : INotificationsService
{
  private readonly INotificationsRepository _notificationsRepo;

  public NotificationsService(INotificationsRepository notificationsRepo)
  {
    _notificationsRepo = notificationsRepo;
  }

  public async Task AddNotificationAsync(string userId, string message)
  {
    var notification = new Notification
    {
      UserId = userId,
      Message = message,
      CreatedAt = DateTime.UtcNow,
      IsRead = false
    };

    await _notificationsRepo.AddNotificationAsync(notification);
  }

  public async Task<IEnumerable<NotificationDTO>> GetNotificationsForUserAsync(string userId)
  {
    var notifications = await _notificationsRepo.GetNotificationsForUserAsync(userId);
    return notifications.Select(n => new NotificationDTO
    {
      Id = n.Id,
      Message = n.Message,
      CreatedAt = n.CreatedAt,
      IsRead = n.IsRead
    }).ToList();
  }

  public async Task MarkNotificationAsReadAsync(int notificationId)
  {
    await _notificationsRepo.MarkAsReadAsync(notificationId);
  }

  public async Task<IEnumerable<NotificationDTO>> GetUnreadNotificationsForUserAsync(string userId)
  {
    var notifications = await _notificationsRepo.GetUnreadNotificationsForUserAsync(userId);
    return notifications.Select(n => new NotificationDTO
    {
      Id = n.Id,
      Message = n.Message,
      CreatedAt = n.CreatedAt,
      IsRead = n.IsRead
    }).ToList();
  }
}
