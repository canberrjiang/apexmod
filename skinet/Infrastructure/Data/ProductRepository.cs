using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
  public class ProductRepository : IProductRepository
  {
    private readonly StoreContext _context;
    public ProductRepository(StoreContext context)
    {
      _context = context;
    }

    public async Task<IReadOnlyList<ProductGraphic>> GetProductGraphicsAsync()
    {
      return await _context.ProductGraphic.ToListAsync();
    }

    public async Task<Product> GetProductByIdAsync(int id)
    {
      return await _context.Products
        .Include(p => p.ProductGraphic)
        .Include(p => p.ProductPlatform)
        .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IReadOnlyList<Product>> GetProductsAsync()
    {
      return await _context.Products
        .Include(p => p.ProductGraphic)
        .Include(p => p.ProductPlatform)
        .ToListAsync();
    }

    public async Task<IReadOnlyList<ProductPlatform>> GetProductPlatformsAsync()
    {
      return await _context.ProductPlatform.ToListAsync();
    }
  }
}