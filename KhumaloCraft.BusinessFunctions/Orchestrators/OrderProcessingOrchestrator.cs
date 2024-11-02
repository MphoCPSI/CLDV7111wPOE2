using KhumaloCraft.Shared.DTOs;
using KhumaloCraft.Shared.Helpers;
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

    await context.CallActivityAsync<Response<string>>("UpdateInventory", cartItemsResponse.Data);

    var ProcessOrderResponse = await context.CallActivityAsync<Response<OrderResponse>>("ProcessOrder", cartItemsResponse.Data);

    if (!ProcessOrderResponse.Success)
    {
      return Response<OrderResponse>.ErrorResponse(ProcessOrderResponse.Message);
    }

    await context.CallActivityAsync("SendOrderStatusNotification", new NotificationRequest
    {
      Status = "Pending",
      UserId = ProcessOrderResponse.Data.UserId,
      OrderId = ProcessOrderResponse.Data.OrderId
    });

    await context.CallActivityAsync("RemoveCart", cartId);

    return Response<OrderResponse>.SuccessResponse(ProcessOrderResponse.Data);
  }
}
