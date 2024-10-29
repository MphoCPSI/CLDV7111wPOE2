using Microsoft.Azure.Functions.Worker;

namespace KhumaloCraft.BusinessFunctions.Activities;

public class SendInventoryUpdateNotificationActivity
{
  [Function("SendInventoryUpdateNotification")]
  public static Task<string> Run([ActivityTrigger] string name)
  {
    return Task.FromResult($"Hello {name}!");
  }
}
