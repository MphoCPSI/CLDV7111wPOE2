using KhumaloCraft.Shared.DTOs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;

namespace KhumaloCraft.BusinessFunctions;

public static class OrderProcessingOrchestrator
{

  [Function("OrderProcessingOrchestrator")]
  public static async Task<Response<OrderResponse>> Run([OrchestrationTrigger] TaskOrchestrationContext context)
  {
    var cartId = context.GetInput<string>();

    var cartItemsResponse = await context.CallActivityAsync<Response<CartDTO>>("GetCartDetails", cartId);

    if (!cartItemsResponse.Success)
    {
      return Response<OrderResponse>.ErrorResponse(cartItemsResponse.Message);
    }

    var ProcessOrderResponse = await context.CallActivityAsync<Response<OrderResponse>>("ProcessOrder", cartItemsResponse.Data);

    if (!ProcessOrderResponse.Success)
    {
      return Response<OrderResponse>.ErrorResponse(ProcessOrderResponse.Message);
    }

    /*     var updateInventoryResponse = await context.CallActivityAsync<Response<string>>("UpdateInventory", cartItemsResponse.Data);

        Console.WriteLine($"UPDATE INVENTORY Success: {updateInventoryResponse.Success}");
        Console.WriteLine($"UPDATE INVENTORY Message: {updateInventoryResponse.Message}");

        if (!updateInventoryResponse.Success)
        {
          return Response<string>.ErrorResponse(updateInventoryResponse.Message);
        }
     */

    context.CallActivityAsync("RemoveCart", cartId);

    return Response<OrderResponse>.SuccessResponse(ProcessOrderResponse.Data);
  }
}
