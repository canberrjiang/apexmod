using System;
using System.Linq.Expressions;
using Core.Entities.OrderAggregate;

namespace Core.Specifications
{
  public class OrdersWithItemsAndOrderingSpecification : BaseSpecification<Order>
  {
    public OrdersWithItemsAndOrderingSpecification(string email) : base(o => o.BuyerEmail == email)
    {
      // Join OrderItem table, join Delivery method table and return order by orderDate in a descending way.
      AddIncludes(o => o.OrderItems);
      AddIncludes(o => o.DeliveryMethod);
      AddOrderByDescending(o => o.OrderDate);
    }

    public OrdersWithItemsAndOrderingSpecification(int id, string email)
        : base(o => o.Id == id && o.BuyerEmail == email)
    {
      AddIncludes(o => o.OrderItems);
      AddIncludes(o => o.DeliveryMethod);
    }
  }
}