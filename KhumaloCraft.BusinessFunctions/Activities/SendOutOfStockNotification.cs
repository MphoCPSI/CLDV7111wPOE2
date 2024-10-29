using Microsoft.Azure.Functions.Worker;

namespace KhumaloCraft.BusinessFunctions.Activities;

public static class SendOutOfStockNotificationActivity
{
  [Function("SendOutOfStockNotification")]
  public static Task<string> Run([ActivityTrigger] string name)
  {
    return Task.FromResult($"Hello {name}!");
  }
}
