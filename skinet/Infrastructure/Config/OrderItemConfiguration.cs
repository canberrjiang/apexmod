using Core.Entities.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config
{
  public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
  {
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
      // Configure the relationship between OrderItem and ItemOrdered
      // OrderItem owns ItemOrdered entity
      builder.OwnsOne(i => i.ItemOrdered, io => { io.WithOwner(); });
      // Override the default Price data type so it will be decimal
      builder.Property(i => i.Price)
        .HasColumnType("decimal(18,2)");
    }
  }
}