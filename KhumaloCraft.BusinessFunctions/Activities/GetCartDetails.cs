using Microsoft.Azure.Functions.Worker;

namespace KhumaloCraft.BusinessFunctions.Activities;

public static class GetCartDetailsActivity
{
  [Function("GetCartDetails")]
  public static Task<string> Run([ActivityTrigger] string cartId)
  {
    Console.WriteLine("CartId: {0}", cartId);
    return Task.FromResult($"fetching the cart: {cartId}");
  }
}
