using Microsoft.Azure.Functions.Worker;

namespace KhumaloCraft.BusinessFunctions.Activities;

public static class UpdateInventoryActivity
{
  [Function("UpdateInventory")]
  public static Task<string> Run([ActivityTrigger] string name)
  {
    Console.WriteLine("activity name: {0}", name);
    return Task.FromResult($"Hello {name}!");
  }
}
