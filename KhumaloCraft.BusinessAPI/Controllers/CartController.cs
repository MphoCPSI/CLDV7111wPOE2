using KhumaloCraft.Business.Services;
using KhumaloCraft.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace KhumaloCraft.BusinessAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class CartController : ControllerBase
  {
    private readonly CartService _cartService;

    public CartController(CartService cartService)
    {
      _cartService = cartService;
    }

    [HttpGet("{cartId}")]
    public ActionResult<CartDTO> GetCart(string cartId)
    {
      var cartDTO = _cartService.GetCartById(cartId);
      if (cartDTO == null)
      {
        return NotFound();
      }

      return Ok(cartDTO);
    }

    [HttpPost("add")]
    public IActionResult AddToCart(string cartId, [FromBody] CartItemDTO item)
    {
      _cartService.AddToCart(cartId, item.ProductId, item.ProductName, item.Price);
      return Ok();
    }

    [HttpPost("link")]
    public IActionResult LinkCartToUser([FromBody] CartLinkDTO linkData)
    {
      _cartService.LinkCartToUser(linkData.cartId, linkData.userId);
      return Ok();
    }

    [HttpPost("clear")]
    public IActionResult ClearCart([FromBody] CartRequestDTO request)
    {
      _cartService.ClearCart(request.CartId);
      return NoContent();
    }
  }
}
