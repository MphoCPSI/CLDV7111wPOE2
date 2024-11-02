using Microsoft.EntityFrameworkCore;
using KhumaloCraft.Data.Data;
using KhumaloCraft.Data.Entities;
using KhumaloCraft.Data.Repositories.Interfaces;

namespace KhumaloCraft.Data.Repositories.Implementations;

public class OrderRepository : IOrderRepository
{
  private readonly ApplicationDbContext _dbContext;

  public OrderRepository(ApplicationDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public async Task<List<Order>> GetAllOrdersAsync()
  {
    return await _dbContext.Orders
      .Include(o => o.OrderItems)
      .ThenInclude(oi => oi.Product)
      .Include(o => o.User)
      .Include(o => o.Status)
      .OrderByDescending(o => o.OrderDate)
      .ToListAsync();
  }

  public async Task<List<Order>> GetOrdersByUserIdAsync(string userId)
  {
    return await _dbContext.Orders
        .Include(o => o.OrderItems)
        .ThenInclude(oi => oi.Product)
        .Include(o => o.Status)
        .Where(o => o.UserId == userId)
        .OrderByDescending(o => o.OrderDate)
        .ToListAsync();
  }

  public async Task<Order> GetOrderByIdAsync(int orderId)
  {
    return await _dbContext.Orders
        .Include(o => o.OrderItems)
        .ThenInclude(oi => oi.Product)
        .Include(o => o.Status)
        .FirstOrDefaultAsync(o => o.OrderId == orderId);
  }

  public async Task AddOrderAsync(Order order)
  {
    await _dbContext.Orders.AddAsync(order);
  }

  public async Task CancelOrderAsync(Order order)
  {
    _dbContext.Orders.Remove(order);
  }

  public async Task<Order> UpdateOrderStatusAsync(int orderId, int statusId)
  {
    var order = await _dbContext.Orders.Include(o => o.Status).FirstOrDefaultAsync(o => o.OrderId == orderId);

    if (order == null)
    {
      throw new KeyNotFoundException($"Order with ID {orderId} not found.");
    }

    var statusExists = await _dbContext.Status.AnyAsync(s => s.StatusId == statusId);

    if (!statusExists)
    {
      throw new InvalidOperationException($"Invalid StatusId {statusId}. No matching status found.");
    }

    order.StatusId = statusId;

    await SaveChangesAsync();

    await _dbContext.Entry(order).Reference(o => o.Status).LoadAsync();

    return order;
  }

  public async Task SaveChangesAsync()
  {
    await _dbContext.SaveChangesAsync();
  }
}
