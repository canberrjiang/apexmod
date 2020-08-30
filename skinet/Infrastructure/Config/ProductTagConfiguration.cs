using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config
{
  public class ProductTagConfiguration : IEntityTypeConfiguration<ProductTag>
  {
    public void Configure(EntityTypeBuilder<ProductTag> builder)
    {
      builder.HasOne(pt => pt.BaseProduct).WithMany(pt => pt.ProductTag).HasForeignKey(pt => pt.ProductId);
      builder.HasOne(pt => pt.Tag).WithMany(pt => pt.ProductTag).HasForeignKey(pt => pt.TagId);
    }
  }
}