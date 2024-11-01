using System.Text.Json;
using KhumaloCraft.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KhumaloCraft.Web.Pages
{
  public class CheckoutModel : PageModel
  {
    public OrderDisplayDTO Order { get; set; } = new OrderDisplayDTO();

    private readonly HttpClient _httpClient;

    public CheckoutModel(IHttpClientFactory httpClientFactory)
    {
      _httpClient = httpClientFactory.CreateClient("BusinessAPI");
    }

    public async Task<IActionResult> OnGet()
    {
      Console.WriteLine("TempData OrderId: " + (TempData["OrderId"] ?? "null"));

      if (TempData["OrderId"] != null)
      {
        var orderIdString = TempData["OrderId"].ToString();
        Console.WriteLine("Parsed OrderId: " + orderIdString);

        if (int.TryParse(orderIdString, out int orderId))
        {
          Console.WriteLine($"Fetching order with ID: {orderId}");
          var response = await _httpClient.GetAsync($"api/orders/{orderId}");

          if (response.IsSuccessStatusCode)
          {
            Order = await response.Content.ReadFromJsonAsync<OrderDisplayDTO>();
            var orderJson = JsonSerializer.Serialize(Order, new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine("Order retrieved successfully. {0}", orderJson);
          }
          else
          {
            TempData["ErrorMessage"] = "Order not found.";
            return NotFound();
          }
        }
        else
        {
          TempData["ErrorMessage"] = "Invalid Order ID.";
          Console.WriteLine("Failed to parse OrderId.");
        }
      }
      else
      {
        Console.WriteLine("OrderId is not set in TempData.");
      }

      return Page();
    }
  }
}
