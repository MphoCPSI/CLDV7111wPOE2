using KhumaloCraft.Business.Services;
using KhumaloCraft.Shared.DTOs;
using Microsoft.Azure.Functions.Worker;

namespace KhumaloCraft.BusinessFunctions.Activities;

public class GetCartDetailsActivity
{
  private readonly CartService _cartService;

  public GetCartDetailsActivity(CartService cartService)
  {
    _cartService = cartService;
  }

  [Function("GetCartDetails")]
  public async Task<CartDTO> Run([ActivityTrigger] string cartId)
  {
    var cart = _cartService.GetCartById(cartId);

    if (cart == null)
    {
      return null;
    }

    return await Task.FromResult(cart);
  }
}
