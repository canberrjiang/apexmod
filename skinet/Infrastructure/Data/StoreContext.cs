using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
  public class StoreContext : DbContext
  {
    // Constructor
    public StoreContext(DbContextOptions<StoreContext> options) : base(options)
    {
    }

    // Properties
    public DbSet<Product> Products { get; set; }
  }
}