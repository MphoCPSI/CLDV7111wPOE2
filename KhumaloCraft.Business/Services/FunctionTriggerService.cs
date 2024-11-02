using System.Text;
using System.Text.Json;
using KhumaloCraft.Business.Interfaces;
using KhumaloCraft.Shared.DTOs;
using KhumaloCraft.Shared.Helpers;
using Microsoft.Extensions.Configuration;

namespace KhumaloCraft.Business.Services;

public class FunctionTriggerService : IFunctionTriggerService
{
  private readonly HttpClient _httpClient;
  private readonly IConfiguration _configuration;

  public FunctionTriggerService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
  {
    _httpClient = httpClientFactory.CreateClient();
    _configuration = configuration;
  }

  public async Task<Response<string>> StartOrderProcessingOrchestratorAsync(CartRequestDTO requestDTO)
  {
    var functionUrl = _configuration["AzureFunctions:OrderProcessingOrchestratorUrl"];
    return await TriggerFunctionAsync(functionUrl, requestDTO);
  }

  public async Task<Response<string>> StartNotificationsOrchestratorAsync(NotificationRequest requestDTO)
  {
    var functionUrl = _configuration["AzureFunctions:NotificationsOrchestratorUrl"];
    return await TriggerFunctionAsync(functionUrl, requestDTO);
  }

  public async Task<Response<string>> StartProductNotificationsOrchestratorAsync(ProductNotificationsRequest requestDTO)
  {
    var functionUrl = _configuration["AzureFunctions:ProductNotificationsOrchestratorUrl"];
    return await TriggerFunctionAsync(functionUrl, requestDTO);
  }

  private async Task<Response<string>> TriggerFunctionAsync(string functionUrl, object payload)
  {
    var jsonPayload = JsonSerializer.Serialize(payload);
    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

    try
    {
      _httpClient.DefaultRequestHeaders.Add("x-functions-key", _configuration["AzureFunctions:XFunctionsKey"]);
      var response = await _httpClient.PostAsync(functionUrl, content);

      if (response.IsSuccessStatusCode)
      {
        var result = await response.Content.ReadAsStringAsync();
        return Response<string>.SuccessResponse(result);
      }
      else
      {
        var errorMessage = await response.Content.ReadAsStringAsync();
        return Response<string>.ErrorResponse($"Failed to trigger function at {functionUrl}: {response.ReasonPhrase}. Details: {errorMessage}");
      }
    }
    catch (Exception ex)
    {
      return Response<string>.ErrorResponse($"An error occurred while triggering the function: {ex.Message}");
    }
  }
}
