using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;

namespace KhumaloCraft.BusinessFunctions;

public static class OrderProcessingOrchestrator
{
  [Function("OrderProcessingOrchestrator")]
  public static async Task Run([OrchestrationTrigger] TaskOrchestrationContext context)
  {
    var cartId = context.GetInput<string>();

    await context.CallActivityAsync("GetCartDetails", cartId);
    await context.CallActivityAsync("ProcessPayment", cartId);
    await context.CallActivityAsync("UpdateInventory", cartId);
  }
}
