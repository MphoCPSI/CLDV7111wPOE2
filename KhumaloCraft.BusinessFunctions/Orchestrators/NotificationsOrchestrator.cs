using KhumaloCraft.Shared.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;

namespace KhumaloCraft.BusinessFunctions;

public static class NotificationsOrchestrator
{
  [Function("NotificationsOrchestrator")]
  public static async Task Run([OrchestrationTrigger] TaskOrchestrationContext context)
  {
    var request = context.GetInput<NotificationRequest>();
    await context.CallActivityAsync<string>("SendOrderStatusNotification", request);
  }
}
