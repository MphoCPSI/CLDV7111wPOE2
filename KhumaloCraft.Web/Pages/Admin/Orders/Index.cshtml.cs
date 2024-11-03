using System.Text;
using System.Text.Json;
using KhumaloCraft.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KhumaloCraft.Web.Pages.Admin.Orders
{
  public class IndexModel : PageModel
  {
    private readonly HttpClient _httpClient;
    private readonly ILogger<IndexModel> _logger;

    public List<OrderDisplayDTO> Orders { get; set; } = new List<OrderDisplayDTO>();
    public List<StatusDTO> StatusList { get; set; } = new List<StatusDTO>();
    [BindProperty]
    public StatusDTO Status { get; set; }

    public IndexModel(IHttpClientFactory httpClientFactory, ILogger<IndexModel> logger)
    {
      _httpClient = httpClientFactory.CreateClient("BusinessAPI");
      _logger = logger;
    }

    public async Task OnGetAsync()
    {
      _logger.LogInformation("Loading orders and status...");
      await LoadOrdersAsync();
      await LoadStatusAsync();
    }

    public async Task<IActionResult> OnPostUpdateStatusAsync()
    {
      var requestBody = await new StreamReader(Request.Body).ReadToEndAsync();

      try
      {
        var payload = JsonSerializer.Deserialize<Dictionary<string, int>>(requestBody);
        if (payload != null && payload.TryGetValue("orderId", out var orderId) && payload.TryGetValue("statusId", out var statusId))
        {
          _logger.LogInformation($"Attempting to update order ID {orderId} with new status ID {statusId}");

          var statusDTO = new StatusDTO { StatusId = statusId };
          var jsonStatus = JsonSerializer.Serialize(statusDTO);
          var content = new StringContent(jsonStatus, Encoding.UTF8, "application/json");

          var response = await _httpClient.PutAsync($"/api/orders/{orderId}/status", content);

          if (response.IsSuccessStatusCode)
          {
            _logger.LogInformation($"Successfully updated order ID {orderId} with status ID {statusId}");
            return new ContentResult { Content = "Status updated successfully", StatusCode = 200 };
          }
          else
          {
            _logger.LogError($"Failed to update order ID {orderId}. Response status: {response.StatusCode}");
            return new BadRequestObjectResult("Error updating status.");
          }
        }
        else
        {
          _logger.LogError("Invalid payload structure.");
          return BadRequest("Invalid request structure.");
        }
      }
      catch (JsonException ex)
      {
        _logger.LogError("Failed to parse JSON: " + ex.Message);
        return BadRequest("Invalid JSON format.");
      }
    }


    private async Task LoadOrdersAsync()
    {
      var response = await _httpClient.GetAsync($"api/orders");

      if (response.IsSuccessStatusCode)
      {
        var jsonResponse = await response.Content.ReadAsStringAsync();
        Orders = JsonSerializer.Deserialize<List<OrderDisplayDTO>>(jsonResponse, new JsonSerializerOptions
        {
          PropertyNameCaseInsensitive = true
        });
        _logger.LogInformation($"Loaded {Orders.Count} orders.");
      }
      else
      {
        _logger.LogError("Failed to load orders from API.");
        Orders = new List<OrderDisplayDTO>();
      }
    }

    private async Task LoadStatusAsync()
    {
      var response = await _httpClient.GetAsync("/api/status");

      if (response.IsSuccessStatusCode)
      {
        var jsonResponse = await response.Content.ReadAsStringAsync();
        StatusList = JsonSerializer.Deserialize<List<StatusDTO>>(jsonResponse, new JsonSerializerOptions
        {
          PropertyNameCaseInsensitive = true
        });
        _logger.LogInformation($"Loaded {StatusList.Count} status options.");
      }
      else
      {
        _logger.LogError("Failed to load status options from API.");
      }
    }
  }
}
