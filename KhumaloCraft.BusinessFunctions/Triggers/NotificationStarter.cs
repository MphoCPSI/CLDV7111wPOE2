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

    string instanceId = await client.ScheduleNewOrchestrationInstanceAsync("OrderProcessingOrchestrator");

    var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
    await response.WriteStringAsync($"Orchestration started with ID = '{instanceId}'.");

    return response;
  }
}
