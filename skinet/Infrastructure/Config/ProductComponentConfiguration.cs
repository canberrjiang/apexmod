using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config
{
  public class ProductComponentConfiguration : IEntityTypeConfiguration<ProductComponent>
  {
    public void Configure(EntityTypeBuilder<ProductComponent> builder)
    {
      builder.Property(pc => pc.Id).IsRequired();
      builder.Property(pc => pc.Title).IsRequired().HasMaxLength(100);
      builder.Property(pc => pc.Description).IsRequired().HasMaxLength(180);
      builder.Property(pc => pc.PPrice).HasColumnType("decimal(18,2)");
      builder.Property(pc => pc.TPrice).HasColumnType("decimal(18,2)");
    }
  }
}