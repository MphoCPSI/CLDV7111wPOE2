using KhumaloCraft.Business.Interfaces;
using KhumaloCraft.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KhumaloCraft.BusinessAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class OrdersController : ControllerBase
  {
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
      _orderService = orderService;
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserOrders(string userId)
    {
      var orders = await _orderService.GetOrdersByUserIdAsync(userId);

      if (orders == null || !orders.Any())
      {
        return NotFound("No orders found for this user.");
      }

      return Ok(orders);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllOrders()
    {
      var orders = await _orderService.GetAllOrders();
      return Ok(orders);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateOrder([FromBody] OrderDTO orderDTO)
    {
      await _orderService.AddOrder(orderDTO);
      return Ok("Order added successfully");
    }

    [HttpPut("{orderId}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(int orderId, [FromBody] StatusDTO statusDTO)
    {
      await _orderService.UpdateOrderStatusAsync(orderId, statusDTO.StatusId);
      return Ok("Order status updated successfully");
    }
  }
}
