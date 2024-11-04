using System.Security.Claims;
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

      return cartId;
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

    public async Task<IActionResult> OnPostEmptyCheckoutCartAsync(string cartId)
    {
      try
      {
        var response = await _httpClient.PostAsJsonAsync($"api/cart/clear", new CartRequestDTO { CartId = cartId });

        if (response.IsSuccessStatusCode)
        {
          TempData["ToastMessage"] = "Your cart has been emptied.";
          return RedirectToPage("/Cart");
        }
        else
        {
          TempData["ToastMessage"] = "An error occurred while emptying the cart. Please try again.";
          return Page();
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error clearing the cart for cartId: {CartId}", cartId);
        TempData["ToastMessage"] = "An unexpected error occurred.";
        return Page();
      }
    }

    public async Task<IActionResult> OnPostCheckoutCartAsync(string cartId)
    {
      var payload = new CartRequestDTO { CartId = cartId };
      var jsonPayload = JsonSerializer.Serialize(payload);
      var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

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

        var linkCartData = new CartLinkDTO
        {
          cartId = userId,
          userId = userId
        };

        var cartLinkData = new StringContent(JsonSerializer.Serialize(linkCartData), Encoding.UTF8, "application/json");

        await _httpClient.PostAsync("api/cart/link", cartLinkData);
      }

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
                TempData["OrderId"] = pollingResult.Data;
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
        Console.WriteLine(ex.ToString());
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

          var orchestrationStatus = JsonSerializer.Deserialize<OrchestrationStatus>(statusContent);

          if (orchestrationStatus != null)
          {
            if (orchestrationStatus.RuntimeStatus == "Completed")
            {
              if (orchestrationStatus.Output?.Success == true)
              {
                var orderId = orchestrationStatus.Output.Data?.OrderId;

                if (!string.IsNullOrEmpty(orderId))
                {
                  return Response<string>.SuccessResponse(orderId);
                }
                else
                {
                  return Response<string>.ErrorResponse("OrderId is missing in the response.");
                }
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
