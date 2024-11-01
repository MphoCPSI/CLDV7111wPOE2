using KhumaloCraft.Shared.DTOs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace KhumaloCraft.BusinessFunctions.OrderProcessing;

public class OrderProcessingStarter
{
  private readonly ILogger<OrderProcessingStarter> _logger;
  public OrderProcessingStarter(ILogger<OrderProcessingStarter> logger)
  {
    _logger = logger;
  }

  [Function("StartOrderOrchestration")]
  public async Task<HttpResponseData> InitiateOrder([HttpTrigger(AuthorizationLevel.Function, "post", Route = "start-order-orchestration")] HttpRequestData req, [DurableClient] DurableTaskClient client)
  {
    _logger.LogInformation("Starting the Order orchestration.");

    var cartRequestDTO = await req.ReadFromJsonAsync<CartRequestDTO>();

    if (cartRequestDTO == null || string.IsNullOrEmpty(cartRequestDTO.CartId))
    {
      var badResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
      await badResponse.WriteStringAsync("Invalid or missing cartId.");
      return badResponse;
    }

    string instanceId = await client.ScheduleNewOrchestrationInstanceAsync("OrderProcessingOrchestrator", cartRequestDTO.CartId);

    string hostname = Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME");

    string baseUrl;

    if (string.IsNullOrEmpty(hostname) || hostname.Contains("localhost"))
    {
      // Fallback for local development
      baseUrl = "http://localhost:7071";
    }
    else
    {
      // For production, ensure the base URL starts with https
      baseUrl = hostname.StartsWith("https://")
          ? hostname
          : "https://" + hostname; // Use https for production
    }

    string statusQueryGetUri = $"{baseUrl}/runtime/webhooks/durabletask/instances/{instanceId}";

    var response = req.CreateResponse(System.Net.HttpStatusCode.Accepted);
    await response.WriteAsJsonAsync(new
    {
      instanceId,
      statusQueryGetUri
    });

    return response;
  }
}
