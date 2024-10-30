using System.Text;
using System.Text.Json;
using KhumaloCraft.Business.Interfaces;
using KhumaloCraft.Shared.DTOs;
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

  public async Task<string> StartOrderProcessingOrchestratorAsync(CartRequestDTO requestDTO)
  {
    var functionUrl = _configuration["AzureFunctions:OrderProcessingOrchestratorUrl"];
    return await TriggerFunctionAsync(functionUrl, requestDTO);
  }

  private async Task<string> TriggerFunctionAsync(string functionUrl, object payload)
  {
    var jsonPayload = JsonSerializer.Serialize(payload);
    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

    var response = await _httpClient.PostAsync(functionUrl, content);

    if (response.IsSuccessStatusCode)
    {
      return await response.Content.ReadAsStringAsync();
    }

    throw new Exception($"Failed to trigger function at {functionUrl}: {response.ReasonPhrase}");

  }

}
