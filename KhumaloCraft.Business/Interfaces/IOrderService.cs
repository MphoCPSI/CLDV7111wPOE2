using KhumaloCraft.Shared.DTOs;

namespace KhumaloCraft.Business.Interfaces;

public interface IOrderService
{
  Task<List<OrderDisplayDTO>> GetAllOrders();
  Task<OrderDisplayDTO?> GetOrderById(int orderId);
  Task<List<OrderDisplayDTO>> GetOrdersByUserIdAsync(string userId);
  Task<string> AddOrder(OrderDTO orderDTO);
  Task UpdateOrderStatusAsync(int orderId, int statusId);
  Task CancelOrder(int orderId);
}
