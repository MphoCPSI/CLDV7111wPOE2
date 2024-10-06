using KhumaloCraft.Data.Entities;

namespace KhumaloCraft.Data.Repositories.Interfaces;

public interface IOrderRepository
{
  Task<List<Order>> GetOrdersByUserIdAsync(string userId);
  Task<List<Order>> GetAllOrdersAsync();
  Task<Order?> GetOrderByIdAsync(int orderId);
  Task AddOrderAsync(Order order);
  Task CancelOrderAsync(Order order);
  Task SaveChangesAsync();
}
