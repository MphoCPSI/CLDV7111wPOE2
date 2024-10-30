using System.Text;
using System.Text.Json;
using KhumaloCraft.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KhumaloCraft.Web.Pages
{
  public class CartModel : PageModel
  {
    private readonly ILogger<CartModel> _logger;
    private readonly HttpClient _httpClient;
    public CartDTO Cart { get; set; } = new CartDTO();

    public CartModel(ILogger<CartModel> logger, IHttpClientFactory httpClientFactory)
    {
      _logger = logger;
      _httpClient = httpClientFactory.CreateClient("BusinessAPI");
    }

    public string GetCartId()
    {
      return Request.Cookies["CartId"];
    }

    public async Task OnGetAsync()
    {
      string cartId = GetCartId();

      if (string.IsNullOrEmpty(cartId))
      {
        Cart = new CartDTO();
        return;
      }

      var response = await _httpClient.GetAsync($"api/cart/{cartId}");

      if (response.IsSuccessStatusCode)
      {
        var jsonResponse = await response.Content.ReadAsStringAsync();

        Cart = JsonSerializer.Deserialize<CartDTO>(jsonResponse, new JsonSerializerOptions
        {
          PropertyNameCaseInsensitive = true
        });
      }
      else
      {
        Cart = new CartDTO();
      }
    }

    public async Task<IActionResult> OnPostCheckoutCartAsync(string cartId)
    {
      var payload = new CartRequestDTO { CartId = cartId };
      var jsonPayload = JsonSerializer.Serialize(payload);
      var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

      var response = await _httpClient.PostAsync($"api/orders/create", content);

      if (!response.IsSuccessStatusCode)
      {
        return Page();
      }

      var jsonResponse = await response.Content.ReadAsStringAsync();
      var result = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonResponse);

      return RedirectToPage("/Checkout");
    }
  }
}
