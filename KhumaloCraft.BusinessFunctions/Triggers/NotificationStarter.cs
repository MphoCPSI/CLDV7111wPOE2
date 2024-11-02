using System.Text.Json;
using KhumaloCraft.Shared.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;


namespace KhumaloCraft.BusinessFunctions.Notifications;

public class NotificationsStarter
{
  private readonly ILogger<NotificationsStarter> _logger;
  public NotificationsStarter(ILogger<NotificationsStarter> logger)
  {
    _logger = logger;
  }

  [Function("StartNotificationOrchestration")]
  public async Task<HttpResponseData> InitiateNotifications([HttpTrigger(AuthorizationLevel.Function, "post", Route = "start-notification-orchestration")] HttpRequestData req, [DurableClient] DurableTaskClient client)
  {
    _logger.LogInformation("Starting the Order orchestration.");

    NotificationRequest payload;

    // Read and parse the JSON body from the request
    try
    {
      var requestBody = await req.ReadAsStringAsync();

      // Check if the requestBody is empty
      if (string.IsNullOrWhiteSpace(requestBody))
      {
        _logger.LogWarning("Request body is empty.");
        var badRequestResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
        await badRequestResponse.WriteStringAsync("Request body cannot be empty.");
        return badRequestResponse;
      }

      // Attempt to deserialize the request body
      payload = JsonSerializer.Deserialize<NotificationRequest>(requestBody);
    }
    catch (JsonException ex)
    {
      _logger.LogError(ex, "Failed to parse JSON in request body.");
      var badRequestResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
      await badRequestResponse.WriteStringAsync("Invalid JSON format in request body.");
      return badRequestResponse;
    }

    string instanceId = await client.ScheduleNewOrchestrationInstanceAsync("NotificationsOrchestrator", payload);

    var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
    await response.WriteStringAsync($"Orchestration started with ID = '{instanceId}'.");

    return response;
  }

  [Function("StartProductNotificationOrchestration")]
  public async Task<HttpResponseData> InitiateProductNotifications([HttpTrigger(AuthorizationLevel.Function, "post", Route = "start-product-notification-orchestration")] HttpRequestData req, [DurableClient] DurableTaskClient client)
  {
    _logger.LogInformation("Starting the Order orchestration.");

    ProductNotificationsRequest payload;

    // Read and parse the JSON body from the request
    try
    {
      var requestBody = await req.ReadAsStringAsync();

      // Check if the requestBody is empty
      if (string.IsNullOrWhiteSpace(requestBody))
      {
        _logger.LogWarning("Request body is empty.");
        var badRequestResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
        await badRequestResponse.WriteStringAsync("Request body cannot be empty.");
        return badRequestResponse;
      }

      // Attempt to deserialize the request body
      payload = JsonSerializer.Deserialize<ProductNotificationsRequest>(requestBody);
    }
    catch (JsonException ex)
    {
      _logger.LogError(ex, "Failed to parse JSON in request body.");
      var badRequestResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
      await badRequestResponse.WriteStringAsync("Invalid JSON format in request body.");
      return badRequestResponse;
    }

    string instanceId = await client.ScheduleNewOrchestrationInstanceAsync("ProductNotificationsOrchestrator", payload);

    var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
    await response.WriteStringAsync($"Orchestration started with ID = '{instanceId}'.");

    return response;
  }
}
