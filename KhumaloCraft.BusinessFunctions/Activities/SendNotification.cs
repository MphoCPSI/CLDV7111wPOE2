using KhumaloCraft.Business.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using KhumaloCraft.Shared.Helpers;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace KhumaloCraft.BusinessFunctions.Notifications
{
  public class SendNotificationActivity
  {
    private readonly INotificationsService _notificationsService;
    private readonly ILogger<SendNotificationActivity> _logger;
    private readonly HttpClient _httpClient;
    private readonly string? _businessApiUrl;

    public SendNotificationActivity(INotificationsService notificationsService, ILogger<SendNotificationActivity> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
      _notificationsService = notificationsService;
      _logger = logger;
      _httpClient = httpClientFactory.CreateClient();
      _businessApiUrl = configuration["ConnectionStrings:BusinessAPI"];

    }

    [Function("SendOrderStatusNotification")]
    public async Task RunSendOrderStatusNotification([ActivityTrigger] NotificationRequest request)
    {
      _logger.LogInformation($"Storing notification for user {request.UserId} with status {request.Status}");

      // Use the service to add the notification
      await _notificationsService.AddNotificationAsync(request.UserId, $"Status for order: {request.OrderId} changed to {request.Status}");

      var jsonPayload = JsonSerializer.Serialize(request);
      _logger.LogInformation($"Serialized JSON Payload: {jsonPayload}");
      var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

      var apiUrl = $"{_businessApiUrl}/api/notifications/notification-order-status";

      try
      {
        var response = await _httpClient.PostAsync(apiUrl, content);
        response.EnsureSuccessStatusCode();
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error sending notification: {ex.Message}");
      }
    }

    [Function("SendProductStatusNotification")]
    public async Task RunSendProductNotification([ActivityTrigger] ProductNotificationsRequest request)
    {
      await _notificationsService.AddNotificationToAllUsersAsync($"{request.ProductName} - {request.Message}");

      var jsonPayload = JsonSerializer.Serialize(request);
      var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

      var apiUrl = $"{_businessApiUrl}/api/notifications/notification-product-update";

      try
      {
        var response = await _httpClient.PostAsync(apiUrl, content);
        response.EnsureSuccessStatusCode();
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error sending notification: {ex.Message}");
      }
    }
  }
}
