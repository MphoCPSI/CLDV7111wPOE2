using Microsoft.Azure.Functions.Worker;

namespace KhumaloCraft.BusinessFunctions.Activities;

public class ProcessPaymentActivity
{
  [Function("ProcessPayment")]
  public static Task<string> Run([ActivityTrigger] string name)
  {
    return Task.FromResult($"Hello {name}!");
  }
}
