using KhumaloCraft.Business.Services;
using KhumaloCraft.Shared.DTOs;
using Microsoft.Azure.Functions.Worker;

namespace KhumaloCraft.BusinessFunctions.Activities
{
  public class GetCartDetailsActivity
  {
    private readonly CartService _cartService;

    public GetCartDetailsActivity(CartService cartService)
    {
      _cartService = cartService;
    }

    [Function("GetCartDetails")]
    public async Task<Response<CartDTO>> Run([ActivityTrigger] string cartId)
    {
      if (string.IsNullOrEmpty(cartId))
      {
        return Response<CartDTO>.ErrorResponse("Cart ID cannot be null or empty.");
      }

      var cart = _cartService.GetCartById(cartId);

      if (cart == null)
      {
        return Response<CartDTO>.ErrorResponse("Cart not found or is empty.");
      }

      return await Task.FromResult(Response<CartDTO>.SuccessResponse(cart));
    }
  }
}
