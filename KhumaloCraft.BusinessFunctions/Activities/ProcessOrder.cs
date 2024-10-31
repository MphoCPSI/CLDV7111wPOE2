using KhumaloCraft.Business.Interfaces;
using KhumaloCraft.Business.Services;
using KhumaloCraft.Shared.DTOs;
using Microsoft.Azure.Functions.Worker;

namespace KhumaloCraft.BusinessFunctions.Activities;

public class ProcessOrderActivity
{
  private readonly IOrderService _orderService;
  private readonly CartService _cartService;

  public ProcessOrderActivity(IOrderService orderService, CartService cartService)
  {
    _orderService = orderService;
    _cartService = cartService;
  }

  [Function("ProcessOrder")]
  public async Task<Response<string>> Run([ActivityTrigger] CartDTO cartDTO)
  {
    if (cartDTO == null || cartDTO.Items.Count == 0)
    {
      return Response<string>.ErrorResponse("Cart not found or empty.");
    }

    try
    {
      var userId = await _cartService.GetUserByCartIdAsync(cartDTO.CartId);

      if (string.IsNullOrEmpty(userId))
      {
        return Response<string>.ErrorResponse("User ID is missing from the cart.");
      }

      var orderDTO = new OrderDTO
      {
        UserId = userId,
        Items = cartDTO.Items.Select(cartItem => new OrderItemDTO
        {
          ProductId = cartItem.ProductId,
          ProductName = cartItem.ProductName,
          Price = cartItem.Price,
          Quantity = cartItem.Quantity
        }).ToList()
      };

      await _orderService.AddOrder(orderDTO);

      return Response<string>.SuccessResponse("Order processed successfully.");
    }
    catch (Exception ex)
    {
      return Response<string>.ErrorResponse($"{ex.Message}");
    }
  }
}
