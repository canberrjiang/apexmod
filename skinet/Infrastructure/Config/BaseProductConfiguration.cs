using Core.Entities;
using Core.Entities.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config
{
  public class BaseProductConfiguration : IEntityTypeConfiguration<BaseProduct>
  {
    public void Configure(EntityTypeBuilder<BaseProduct> builder)
    {
      builder.Property(p => p.Id).IsRequired();
      builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
      builder.Property(p => p.Description).IsRequired();
      builder.Property(p => p.Price).HasColumnType("decimal(18,2)");
      builder.Property(p => p.DiscountPrice).HasColumnType("decimal(18,2)");
      builder.HasOne(p => p.ProductCategory).WithMany(p => p.BaseProduct).HasForeignKey(p => p.ProductCategoryId);
      builder.HasMany(p => p.Photos).WithOne(p => p.Product).OnDelete(DeleteBehavior.Cascade);
    }
  }
}