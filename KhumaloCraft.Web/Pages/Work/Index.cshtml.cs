using System.Security.Claims;
using System.Text;
using System.Text.Json;
using KhumaloCraft.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KhumaloCraft.Pages.Work
{
  public class IndexModel : PageModel
  {
    private readonly ILogger<IndexModel> _logger;
    private readonly HttpClient _httpClient;
    public List<ProductDTO> ProductList { get; set; } = new List<ProductDTO>();

    public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory httpClientFactory)
    {
      _logger = logger;
      _httpClient = httpClientFactory.CreateClient("BusinessAPI");
    }

    public async Task OnGetAsync()
    {
      var response = await _httpClient.GetAsync("api/products");

      if (response.IsSuccessStatusCode)
      {
        var jsonResponse = await response.Content.ReadAsStringAsync();
        ProductList = JsonSerializer.Deserialize<List<ProductDTO>>(jsonResponse, new JsonSerializerOptions
        {
          PropertyNameCaseInsensitive = true
        }) ?? new List<ProductDTO>();
      }
      else
      {
        _logger.LogError("Error fetching products from the API");
      }
    }

    public async Task<IActionResult> OnPostAddToCartAsync(int productId, string productName, decimal price)
    {
      string cartId = GetOrCreateCartId(); // Generate or retrieve the cartId

      var cartItem = new CartItemDTO
      {
        ProductId = productId,
        ProductName = productName,
        Price = price,
        Quantity = 1,
        CartId = cartId
      };

      var jsonContent = new StringContent(JsonSerializer.Serialize(cartItem), Encoding.UTF8, "application/json");

      var response = await _httpClient.PostAsync($"api/cart/add?cartId={cartId}", jsonContent);

      if (response.IsSuccessStatusCode)
      {
        return RedirectToPage("/Work/Index");
      }

      return Page();
    }

    public string GetOrCreateCartId()
    {
      if (Request.Cookies.ContainsKey("CartId"))
      {
        return Request.Cookies["CartId"];
      }

      string? userId = null;
      if (User.Identity?.IsAuthenticated == true)
      {
        var token = Request.Cookies["AuthToken"];
        if (!string.IsNullOrEmpty(token))
        {
          var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
          var jwtToken = tokenHandler.ReadJwtToken(token);
          userId = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
        }
      }

      string cartId = userId ?? Guid.NewGuid().ToString();

      Response.Cookies.Append("CartId", cartId, new CookieOptions
      {
        Expires = DateTimeOffset.UtcNow.AddDays(30),
        HttpOnly = true,
        IsEssential = true
      });

      return cartId;
    }
  }
}
