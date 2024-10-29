using KhumaloCraft.Shared.DTOs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;

namespace KhumaloCraft.BusinessFunctions;

public static class OrderProcessingOrchestrator
{
  [Function("OrderProcessingOrchestrator")]
  public static async Task Run([OrchestrationTrigger] TaskOrchestrationContext context)
  {
    var orderDTO = context.GetInput<OrderDTO>();

    await context.CallActivityAsync("UpdateInventory", orderDTO);
  }
}
