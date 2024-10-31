using KhumaloCraft.Shared.DTOs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;

namespace KhumaloCraft.BusinessFunctions;

public static class OrderProcessingOrchestrator
{

  [Function("OrderProcessingOrchestrator")]
  public static async Task<Response<string>> Run([OrchestrationTrigger] TaskOrchestrationContext context)
  {
    var cartId = context.GetInput<string>();

    var cartItemsResponse = await context.CallActivityAsync<Response<CartDTO>>("GetCartDetails", cartId);

    Console.WriteLine($"GET CART Success: {cartItemsResponse.Success}");
    Console.WriteLine($"GET CART Message: {cartItemsResponse.Message}");

    if (!cartItemsResponse.Success)
    {
      return Response<string>.ErrorResponse(cartItemsResponse.Message);
    }

    var ProcessOrderResponse = await context.CallActivityAsync<Response<string>>("ProcessOrder", cartItemsResponse.Data);

    Console.WriteLine($"PROCESS ORDER Success: {ProcessOrderResponse.Success}");
    Console.WriteLine($"PROCESS ORDER Message: {ProcessOrderResponse.Message}");

    if (!ProcessOrderResponse.Success)
    {
      return Response<string>.ErrorResponse(ProcessOrderResponse.Message);
    }

    var updateInventoryResponse = await context.CallActivityAsync<Response<string>>("UpdateInventory", cartItemsResponse.Data);

    Console.WriteLine($"UPDATE INVENTORY Success: {updateInventoryResponse.Success}");
    Console.WriteLine($"UPDATE INVENTORY Message: {updateInventoryResponse.Message}");

    if (!updateInventoryResponse.Success)
    {
      return Response<string>.ErrorResponse(updateInventoryResponse.Message);
    }

    // Clear the cart

    return Response<string>.SuccessResponse("Order processing completed successfully.");
  }
}
