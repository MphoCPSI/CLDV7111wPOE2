using System.Text;
using System.Text.Json;
using KhumaloCraft.Shared.DTOs;
using KhumaloCraft.Shared.Helpers;
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

      try
      {
        var response = await _httpClient.PostAsync("api/orders/create", content);

        if (!response.IsSuccessStatusCode)
        {
          TempData["ToastMessage"] = "An error occurred. Please try again.";
          return Page();
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();

        var standardResponse = JsonSerializer.Deserialize<Response<string>>(jsonResponse, new JsonSerializerOptions
        {
          PropertyNameCaseInsensitive = true
        });

        if (standardResponse != null && standardResponse.Success && !string.IsNullOrEmpty(standardResponse.Data))
        {
          var orchestrationData = JsonSerializer.Deserialize<OrchestrationStartResponse>(standardResponse.Data, new JsonSerializerOptions
          {
            PropertyNameCaseInsensitive = true
          });

          if (orchestrationData != null)
          {
            if (!string.IsNullOrEmpty(orchestrationData.StatusQueryGetUri))
            {
              var pollingResult = await PollOrchestrationStatusAsync(orchestrationData.StatusQueryGetUri);

              if (pollingResult.Success)
              {
                TempData["ToastMessage"] = pollingResult.Message;
                return RedirectToPage("/Checkout");
              }
              else
              {
                TempData["ToastMessage"] = pollingResult.Message;
                await OnGetAsync();
                return Page();
              }
            }
          }
        }

        TempData["ToastMessage"] = standardResponse?.Message ?? "An error occurred. Please try again.";
        return Page();
      }
      catch (Exception ex)
      {
        await OnGetAsync();
        TempData["ToastMessage"] = "An error occurred. Please try again.";
        return Page();
      }
    }


    private async Task<Response<string>> PollOrchestrationStatusAsync(string statusUrl)
    {
      var maxRetries = 10;
      var delay = TimeSpan.FromSeconds(3);

      for (int i = 0; i < maxRetries; i++)
      {
        var statusResponse = await _httpClient.GetAsync(statusUrl);
        if (statusResponse.IsSuccessStatusCode)
        {
          var statusContent = await statusResponse.Content.ReadAsStringAsync();
          Console.WriteLine("statusContent Status Code: " + statusContent);

          var orchestrationStatus = JsonSerializer.Deserialize<OrchestrationStatus>(statusContent);
          if (orchestrationStatus != null)
          {
            if (orchestrationStatus.RuntimeStatus == "Completed")
            {
              if (orchestrationStatus.Output?.Success == true)
              {
                return Response<string>.SuccessResponse("Your order has been completed successfully.");
              }
              else
              {
                return Response<string>.ErrorResponse(orchestrationStatus.Output?.Message ?? "Order processing encountered an error.");
              }
            }
            else if (orchestrationStatus.RuntimeStatus == "Failed")
            {
              return Response<string>.ErrorResponse("Order processing failed. Please try again.");
            }
          }
        }

        await Task.Delay(delay);
      }

      return Response<string>.ErrorResponse("Order processing is taking longer than expected. Please check again later.");
    }
  }
}
