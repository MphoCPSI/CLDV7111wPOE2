using KhumaloCraft.Shared.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;

namespace KhumaloCraft.BusinessFunctions;

public static class ProductNotificationsOrchestrator
{
  [Function("ProductNotificationsOrchestrator")]
  public static async Task Run([OrchestrationTrigger] TaskOrchestrationContext context)
  {
    var request = context.GetInput<ProductNotificationsRequest>();
    await context.CallActivityAsync<string>("SendProductStatusNotification", request);
  }
}
