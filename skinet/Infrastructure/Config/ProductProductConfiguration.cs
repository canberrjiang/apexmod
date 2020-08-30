using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config
{
  public class ProductProductConfiguration : IEntityTypeConfiguration<ProductProduct>
  {
    public void Configure(EntityTypeBuilder<ProductProduct> builder)
    {
      builder.HasOne(pp => pp.Product).WithMany(pp => pp.ChildProducts);
      builder.HasOne(pp => pp.ChildProduct).WithMany();
    }
  }
}