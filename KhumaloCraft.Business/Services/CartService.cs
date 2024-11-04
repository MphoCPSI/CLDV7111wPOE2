using KhumaloCraft.Business.Interfaces;
using KhumaloCraft.Data.Entities;
using KhumaloCraft.Shared.DTOs;

namespace KhumaloCraft.Business.Services;

public class CartService
{
  private readonly ICartRepository _cartRepository;

  public CartService(ICartRepository cartRepository)
  {
    _cartRepository = cartRepository;
  }

  public CartDTO GetCartById(string cartId)
  {
    var cart = _cartRepository.GetCartById(cartId);

    // Return null if cart doesn't exist
    if (cart == null)
      return null;

    // Map Cart to CartDto
    return new CartDTO
    {
      CartId = cart.CartId,
      Items = cart.Items.Select(item => new CartItemDTO
      {
        CartItemId = item.CartItemId,
        ProductId = item.ProductId,
        ProductName = item.ProductName,
        Price = item.Price,
        Quantity = item.Quantity,
        CartId = cart.CartId
      }).ToList()
    };
  }

  public async Task<string> GetUserByCartIdAsync(string cartId)
  {
    var userId = await _cartRepository.GetUserByCartIdAsync(cartId);
    if (userId == null)
    {
      return null;
    }
    return userId;
  }

  public void AddToCart(string cartId, int productId, string productName, decimal price)
  {
    var cart = _cartRepository.GetCartById(cartId);

    if (cart == null)
    {
      cart = new Cart { CartId = cartId, LastUpdatedAt = DateTime.UtcNow };
      _cartRepository.SaveCart(cart);
    }

    var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);

    if (existingItem != null)
    {
      existingItem.Quantity++;
    }
    else
    {
      cart.Items.Add(new CartItem { ProductId = productId, ProductName = productName, Price = price, Quantity = 1, CartId = cartId });
    }

    cart.LastUpdatedAt = DateTime.UtcNow;
    _cartRepository.SaveCart(cart);
  }

  public void LinkCartToUser(string cartId, string userId)
  {
    _cartRepository.UpdateCartUserId(cartId, userId);
  }

  public void ClearCart(string cartId)
  {
    _cartRepository.DeleteCart(cartId);
  }
}

