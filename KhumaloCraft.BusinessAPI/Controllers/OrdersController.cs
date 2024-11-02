using KhumaloCraft.Business.Interfaces;
using KhumaloCraft.Shared.DTOs;
using KhumaloCraft.Shared.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KhumaloCraft.BusinessAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class OrdersController : ControllerBase
  {
    private readonly IOrderService _orderService;
    private readonly IFunctionTriggerService _functionTriggerService;

    public OrdersController(IOrderService orderService, IFunctionTriggerService functionTriggerService)
    {
      _orderService = orderService;
      _functionTriggerService = functionTriggerService;
    }


    [HttpGet("{orderId}")]
    public async Task<IActionResult> GetOrderById(int orderId)
    {
      var orders = await _orderService.GetOrderById(orderId);

      if (orders == null)
      {
        return NotFound("No orders found for this user.");
      }

      return Ok(orders);
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
    public async Task<IActionResult> CreateOrder([FromBody] CartRequestDTO cartRequestDTO)
    {
      try
      {
        var response = await _functionTriggerService.StartOrderProcessingOrchestratorAsync(cartRequestDTO);

        return Accepted(response);
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { Success = false, Message = $"An internal error occurred: {ex.Message}" });
      }
    }

    [HttpPut("{orderId}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(int orderId, [FromBody] StatusDTO statusDTO)
    {
      var order = await _orderService.UpdateOrderStatusAsync(orderId, statusDTO.StatusId);

      await _functionTriggerService.StartNotificationsOrchestratorAsync(new NotificationRequest
      {
        Status = order.StatusName,
        UserId = order.UserId,
        OrderId = orderId.ToString(),
      });

      return Ok($"Order status updated successfully.");
    }
  }
}
