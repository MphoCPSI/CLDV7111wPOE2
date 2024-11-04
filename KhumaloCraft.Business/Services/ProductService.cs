using KhumaloCraft.Business.Interfaces;
using KhumaloCraft.Data.Entities;
using KhumaloCraft.Data.Repositories.Interfaces;
using KhumaloCraft.Shared.DTOs;

namespace KhumaloCraft.Business.Services;

public class ProductService : IProductService
{
  private readonly IProductRepository _productRepository;

  public ProductService(IProductRepository productRepository)
  {
    _productRepository = productRepository;
  }

  public async Task<List<ProductDTO>> GetAllProducts()
  {
    var products = await _productRepository.GetAllProductsAsync();

    var productDTOs = products.Select(p => new ProductDTO
    {
      ProductId = p.ProductId,
      Name = p.Name,
      Price = p.Price,
      Description = p.Description,
      ImageSrc = p.ImageSrc,
      Quantity = p.Quantity,
      CategoryId = p.CategoryId,
      CategoryName = p.Category?.CategoryName
    }).ToList();

    return productDTOs;
  }

  public async Task<ProductDTO?> GetProductById(int productId)
  {
    var product = await _productRepository.GetProductByIdAsync(productId);
    if (product == null) return null;

    return new ProductDTO
    {
      ProductId = product.ProductId,
      Name = product.Name,
      Price = product.Price,
      Description = product.Description,
      ImageSrc = product.ImageSrc,
      Quantity = product.Quantity,
      CategoryId = product.CategoryId,
      CategoryName = product.Category?.CategoryName
    };
  }

  public async Task AddProduct(ProductDTO productDTO)
  {
    var product = new Product
    {
      Name = productDTO.Name,
      Price = productDTO.Price,
      Description = productDTO.Description,
      ImageSrc = productDTO.ImageSrc,
      Quantity = productDTO.Quantity,
      CategoryId = productDTO.CategoryId
    };

    await _productRepository.AddProductAsync(product);
    await _productRepository.SaveChangesAsync();
  }

  public async Task UpdateProduct(ProductDTO productDTO)
  {
    var product = await _productRepository.GetProductByIdAsync(productDTO.ProductId);
    if (product == null) return;

    product.Name = productDTO.Name;
    product.Price = productDTO.Price;
    product.Description = productDTO.Description;
    product.ImageSrc = productDTO.ImageSrc;
    product.Quantity = productDTO.Quantity;
    product.CategoryId = productDTO.CategoryId;

    await _productRepository.UpdateProductAsync(product);
    await _productRepository.SaveChangesAsync();
  }

  public async Task DeleteProduct(int productId)
  {
    var product = await _productRepository.GetProductByIdAsync(productId);
    if (product == null) return;

    await _productRepository.DeleteProductAsync(product);
    await _productRepository.SaveChangesAsync();
  }

  public async Task UpdateInventory(List<CartItemDTO> cartItems)
  {
    foreach (var item in cartItems)
    {
      var product = await _productRepository.GetProductByIdAsync(item.ProductId);

      if (product == null)
      {
        continue;
      }

      if (product.Quantity < item.Quantity)
      {
        throw new InvalidOperationException($"Insufficient stock for {item.ProductName}");
      }

      product.Quantity -= item.Quantity;
      await _productRepository.UpdateProductAsync(product);
    }

    await _productRepository.SaveChangesAsync();
  }
}
