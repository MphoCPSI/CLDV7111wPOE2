using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;

namespace KhumaloCraft.BusinessFunctions;

public static class OrderProcessingOrchestrator
{
  [Function("OrderProcessingOrchestrator")]
  public static async Task Run([OrchestrationTrigger] TaskOrchestrationContext context)
  {
    await context.CallActivityAsync<string>("UpdateInventory", "Updating Inventory");
  }
}
