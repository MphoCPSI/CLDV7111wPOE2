using KhumaloCraft.Shared.DTOs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;

namespace KhumaloCraft.BusinessFunctions;

public static class OrderProcessingOrchestrator
{
  [Function("OrderProcessingOrchestrator")]
  public static async Task Run([OrchestrationTrigger] TaskOrchestrationContext context)
  {
    var cartId = context.GetInput<string>();

    var cartItems = await context.CallActivityAsync<CartDTO>("GetCartDetails", cartId);
    var cartItemsJson = System.Text.Json.JsonSerializer.Serialize(cartItems);
    Console.WriteLine($"Fetched CartItems: {cartItemsJson}");

    // await context.CallActivityAsync("ProcessPayment", cartItems);

    var updateStatus = await context.CallActivityAsync<string>("UpdateInventory", cartItems);
    Console.WriteLine($"Updated CartItems: {updateStatus}");
  }
}
