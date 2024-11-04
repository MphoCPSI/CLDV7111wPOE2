using System.Security.Claims;
using System.Text.Json;
using KhumaloCraft.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KhumaloCraft.Web.Pages
{
  public class OrdersModel : PageModel
  {
    private readonly HttpClient _httpClient;
    private readonly ILogger<OrdersModel> _logger;
    public List<OrderDisplayDTO> Orders { get; set; } = new List<OrderDisplayDTO>();

    public OrdersModel(ILogger<OrdersModel> logger, IHttpClientFactory httpClientFactory)
    {
      _httpClient = httpClientFactory.CreateClient("BusinessAPI");
      _logger = logger;
    }

    public async Task OnGetAsync()
    {
      var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

      if (string.IsNullOrEmpty(userId))
      {
        Orders = new List<OrderDisplayDTO>();
        return;
      }

      var response = await _httpClient.GetAsync($"api/orders/user/{userId}");

      if (response.IsSuccessStatusCode)
      {
        var jsonResponse = await response.Content.ReadAsStringAsync();

        Orders = JsonSerializer.Deserialize<List<OrderDisplayDTO>>(jsonResponse, new JsonSerializerOptions
        {
          PropertyNameCaseInsensitive = true
        });
      }
      else
      {
        Orders = new List<OrderDisplayDTO>();
      }
    }
  }
}
