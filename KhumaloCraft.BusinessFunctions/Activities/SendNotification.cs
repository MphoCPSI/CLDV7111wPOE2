using KhumaloCraft.Business.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using KhumaloCraft.Shared.Helpers;

namespace KhumaloCraft.BusinessFunctions.Notifications
{
  public class SendNotificationActivity
  {
    private readonly INotificationsService _notificationsService;
    private readonly ILogger<SendNotificationActivity> _logger;

    public SendNotificationActivity(INotificationsService notificationsService, ILogger<SendNotificationActivity> logger)
    {
      _notificationsService = notificationsService;
      _logger = logger;
    }

    [Function("SendOrderStatusNotification")]
    public async Task RunSendOrderStatusNotification([ActivityTrigger] NotificationRequest request)
    {
      _logger.LogInformation($"Storing notification for user {request.UserId} with status {request.Status}");

      // Use the service to add the notification
      await _notificationsService.AddNotificationAsync(request.UserId, $"Status for order: {request.OrderId} changed to {request.Status}");
    }

    [Function("SendProductStatusNotification")]
    public async Task RunSendProductNotification([ActivityTrigger] ProductNotificationsRequest request)
    {
      // Use the service to add the notification
      await _notificationsService.AddNotificationToAllUsersAsync($"{request.ProductName} - {request.Message}");
    }
  }
}
