using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities.OrderAggregate;

namespace Core.Interfaces
{
  public interface IProductRepository
  {
    Task<Product> GetProductByIdAsync(int id);
    Task<IReadOnlyList<Product>> GetProductsAsync();
    Task<IReadOnlyList<ProductGraphic>> GetProductGraphicsAsync();
    Task<IReadOnlyList<ProductPlatform>> GetProductPlatformsAsync();
  }
}