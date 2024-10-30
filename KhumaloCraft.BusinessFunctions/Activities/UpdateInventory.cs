using Microsoft.Azure.Functions.Worker;

namespace KhumaloCraft.BusinessFunctions.Activities;

public static class UpdateInventoryActivity
{
  [Function("UpdateInventory")]
  public static Task Run([ActivityTrigger] string cartId)
  {
    return Task.FromResult($"Updating inventory for CartId: {cartId}");
  }
}
