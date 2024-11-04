using Microsoft.Azure.Functions.Worker;

namespace KhumaloCraft.BusinessFunctions.Activities;

public static class ProcessPaymentActivity
{
  [Function("ProcessPayment")]
  public static Task<string> Run([ActivityTrigger] string cartId)
  {
    Console.WriteLine("CartId: {0}", cartId);
    return Task.FromResult($"Processing Payment for CartId: {cartId}");
  }
}
