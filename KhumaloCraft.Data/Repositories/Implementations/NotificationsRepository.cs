using Microsoft.EntityFrameworkCore;
using KhumaloCraft.Data.Data;
using KhumaloCraft.Data.Entities;
using KhumaloCraft.Data.Repositories.Interfaces;

namespace KhumaloCraft.Data.Repositories.Implementations;

public class NotificationsRepository : INotificationsRepository
{
  private readonly ApplicationDbContext _dbContext;

  public NotificationsRepository(ApplicationDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public async Task AddNotificationAsync(Notification notification)
  {
    await _dbContext.Notifications.AddAsync(notification);
    await _dbContext.SaveChangesAsync();
  }

  public async Task<IEnumerable<Notification>> GetNotificationsForUserAsync(string userId)
  {
    return await _dbContext.Notifications
        .Where(n => n.UserId == userId)
        .OrderByDescending(n => n.CreatedAt)
        .ToListAsync();
  }

  public async Task MarkAsReadAsync(int notificationId)
  {
    var notification = await _dbContext.Notifications.FindAsync(notificationId);
    if (notification != null)
    {
      notification.IsRead = true;
      _dbContext.Notifications.Update(notification);
      await _dbContext.SaveChangesAsync();
    }
  }

  public async Task<IEnumerable<Notification>> GetUnreadNotificationsForUserAsync(string userId)
  {
    return await _dbContext.Notifications
        .Where(n => n.UserId == userId && !n.IsRead)
        .OrderByDescending(n => n.CreatedAt)
        .ToListAsync();
  }
}
