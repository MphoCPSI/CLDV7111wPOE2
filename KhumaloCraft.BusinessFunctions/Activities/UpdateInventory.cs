using KhumaloCraft.Business.Interfaces;
using KhumaloCraft.Shared.DTOs;
using Microsoft.Azure.Functions.Worker;

namespace KhumaloCraft.BusinessFunctions.Activities
{
  public class UpdateInventoryActivity
  {
    private readonly IProductService _productService;

    public UpdateInventoryActivity(IProductService productService)
    {
      _productService = productService;
    }

    [Function("UpdateInventory")]
    public async Task<Response<string>> Run([ActivityTrigger] CartDTO cartDTO)
    {
      if (cartDTO == null || cartDTO.Items.Count == 0)
      {
        return Response<string>.ErrorResponse("Cart not found or empty.");
      }

      try
      {
        await _productService.UpdateInventory(cartDTO.Items);
        return Response<string>.SuccessResponse("Inventory updated successfully.");
      }
      catch (Exception ex)
      {
        return Response<string>.ErrorResponse($"{ex.Message}");
      }
    }
  }
}
