using System.Text;
using System.Text.Json;
using KhumaloCraft.Shared.DTOs;
using Microsoft.Azure.Functions.Worker;

namespace KhumaloCraft.BusinessFunctions.Activities;

public static class UpdateInventoryActivity
{
  [Function("UpdateInventory")]
  public static Task Run([ActivityTrigger] OrderDTO orderDTO)
  {
    Console.WriteLine("OrderDTO: {0}", JsonSerializer.Serialize(orderDTO));
    return Task.CompletedTask;
  }
}
