using KhumaloCraft.Business.Interfaces;
using KhumaloCraft.Data.Data;
using KhumaloCraft.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace KhumaloCraft.Data.Repositories.Implementations;

public class CartRepository : ICartRepository
{
  private readonly ApplicationDbContext _dbContext;

  public CartRepository(ApplicationDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public Cart GetCartById(string cartId)
  {
    return _dbContext.Carts.Include(c => c.Items).FirstOrDefault(c => c.CartId == cartId || c.UserId == cartId);
  }

  public async Task<string> GetUserByCartIdAsync(string cartId)
  {
    return await _dbContext.Carts
        .Where(c => c.CartId == cartId)
        .Select(c => c.UserId)
        .FirstOrDefaultAsync();
  }

  public void SaveCart(Cart cart)
  {
    var existingCart = _dbContext.Carts.Find(cart.CartId);
    if (existingCart == null)
    {
      _dbContext.Carts.Add(cart);
    }
    else
    {
      _dbContext.Entry(existingCart).CurrentValues.SetValues(cart);
    }
    _dbContext.SaveChanges();
  }

  public void UpdateCartUserId(string cartId, string userId)
  {
    var cart = _dbContext.Carts.FirstOrDefault(c => c.CartId == cartId);

    if (cart != null)
    {
      cart.UserId = userId;
      _dbContext.SaveChanges();
    }
  }

  public void DeleteCart(string cartId)
  {
    var cart = _dbContext.Carts.Find(cartId);
    if (cart != null)
    {
      _dbContext.Carts.Remove(cart);
      _dbContext.SaveChanges();
    }
  }

  public IEnumerable<Cart> GetCartsOlderThan(DateTime expirationDate)
  {
    return _dbContext.Carts.Where(c => c.LastUpdatedAt < expirationDate).ToList();
  }
}

