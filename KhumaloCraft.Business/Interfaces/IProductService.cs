using KhumaloCraft.Shared.DTOs;

namespace KhumaloCraft.Business.Interfaces;

public interface IProductService
{
  Task<List<ProductDTO>> GetAllProducts();
  Task<ProductDTO?> GetProductById(int productId);
  Task AddProduct(ProductDTO productDTO);
  Task UpdateProduct(ProductDTO productDTO);
  Task DeleteProduct(int productId);
  Task UpdateInventory(List<CartItemDTO> cartItems);
}
