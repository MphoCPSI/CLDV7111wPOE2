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

    public async Task<IActionResult> OnPostUpdateStatusAsync(int orderId, int statusId)
    {
      _logger.LogInformation($"Attempting to update order ID {orderId} with new status ID {statusId}");

      if (!ModelState.IsValid)
      {
        _logger.LogWarning("Model state is invalid.");
        foreach (var modelState in ModelState)
        {
          foreach (var error in modelState.Value.Errors)
          {
            _logger.LogError($"Model Error in {modelState.Key}: {error.ErrorMessage}");
          }
        }
        await LoadOrdersAsync();
        await LoadStatusAsync();
        return Page();
      }

      var statusDTO = new StatusDTO { StatusId = statusId };
      var jsonStatus = JsonSerializer.Serialize(statusDTO);
      var content = new StringContent(jsonStatus, Encoding.UTF8, "application/json");

      var response = await _httpClient.PutAsync($"/api/orders/{orderId}/status", content);

      if (response.IsSuccessStatusCode)
      {
        _logger.LogInformation($"Successfully updated order ID {orderId} with status ID {statusId}");
        return RedirectToPage("/Admin/Orders/Index");
      }

      _logger.LogError($"Failed to update order ID {orderId}. Response status: {response.StatusCode}");
      ModelState.AddModelError(string.Empty, "Error updating status.");
      await LoadOrdersAsync();
      await LoadStatusAsync();
      return Page();
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
