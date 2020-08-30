using System;
using Core.Entities.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config
{
  public class OrderConfiguration : IEntityTypeConfiguration<Order>
  {
    public void Configure(EntityTypeBuilder<Order> builder)
    {
      // Configuration the relationship between order and address
      // Order owns address entity
      builder.OwnsOne(o => o.ShipToAddress, a =>
      {
        a.WithOwner();
      });

      // Convert the OrderStatus enum return value
      // Instead of returning int, we convert the result to string
      builder.Property(s => s.Status)
      .HasConversion(
          o => o.ToString(),
          o => (OrderStatus)Enum.Parse(typeof(OrderStatus), o)
      );
      // Configure the relationship and delete behavior of order and order items
      // One order can have many order items
      // Order items will be deleted as soon as the related order is deleted.
      builder.HasMany(o => o.OrderItems).WithOne().OnDelete(DeleteBehavior.Cascade);
    }
  }
}