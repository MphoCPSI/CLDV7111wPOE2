using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using KhumaloCraft.Shared.DTOs;
using KhumaloCraft.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using KhumaloCraft.Shared.Helpers;

namespace KhumaloCraft.BusinessAPI.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class ProductsController : ControllerBase
  {
    private readonly IProductService _productService;
    private readonly IFunctionTriggerService _functionTriggerService;

    public ProductsController(IProductService productService, IFunctionTriggerService functionTriggerService)
    {
      _productService = productService;
      _functionTriggerService = functionTriggerService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllProducts()
    {
      var products = await _productService.GetAllProducts();

      return Ok(products);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetProductById(int id)
    {
      var product = await _productService.GetProductById(id);
      if (product == null) return NotFound();
      return Ok(product);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddProduct([FromBody] ProductDTO productDTO)
    {
      await _productService.AddProduct(productDTO);
      return Ok("Product added successfully");
    }

    [HttpPatch("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> PatchProduct(int id, [FromBody] JsonPatchDocument<ProductDTO> patchDoc)
    {
      if (patchDoc == null) return BadRequest();

      var productDTO = await _productService.GetProductById(id);
      if (productDTO == null) return NotFound();

      patchDoc.ApplyTo(productDTO);

      if (!TryValidateModel(productDTO))
      {
        return BadRequest(ModelState);
      }

      await _productService.UpdateProduct(productDTO);

      return Ok(productDTO);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDTO productDTO)
    {
      if (id != productDTO.ProductId) return BadRequest("Product ID mismatch");

      await _productService.UpdateProduct(productDTO);

      if (productDTO.SendMessage)
      {
        await _functionTriggerService.StartProductNotificationsOrchestratorAsync(new ProductNotificationsRequest
        {
          ProductName = productDTO.Name,
          Message = productDTO.Message
        });
      }

      return Ok("Product updated successfully");
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
      await _productService.DeleteProduct(id);
      return Ok("Product deleted successfully");
    }
  }
}
