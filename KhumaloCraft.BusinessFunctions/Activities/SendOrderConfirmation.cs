using Microsoft.Azure.Functions.Worker;

namespace KhumaloCraft.BusinessFunctions.Activities;

public class SendOrderStatusActivity
{
  [Function("SendOrderStatus")]
  public static Task<string> Run([ActivityTrigger] string cartId)
  {
    return Task.FromResult($"Hello {cartId}!");
  }
}
