using KhumaloCraft.Business.Services;
using KhumaloCraft.Shared.DTOs;
using Microsoft.Azure.Functions.Worker;

namespace KhumaloCraft.BusinessFunctions.Activities
{
  public class ClearCartActivity
  {
    private readonly CartService _cartService;

    public ClearCartActivity(CartService cartService)
    {
      _cartService = cartService;
    }

    [Function("RemoveCart")]
    public Task Run([ActivityTrigger] string cartId)
    {
      if (string.IsNullOrEmpty(cartId))
      {
        throw new ArgumentException("Cart ID cannot be null or empty.", nameof(cartId));
      }

      return Task.Run(() => _cartService.ClearCart(cartId));
    }
  }
}
