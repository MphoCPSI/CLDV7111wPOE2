using KhumaloCraft.Shared.DTOs;

namespace KhumaloCraft.Business.Interfaces;

public interface IOrderService
{
  Task<List<OrderDisplayDTO>> GetAllOrders();
  Task<OrderDisplayDTO> GetOrderById(int orderId);
  Task<List<OrderDisplayDTO>> GetOrdersByUserIdAsync(string userId);
  Task<int> AddOrder(OrderDTO orderDTO);
  Task<OrderDisplayDTO> UpdateOrderStatusAsync(int orderId, int statusId);
  Task CancelOrder(int orderId);
}
