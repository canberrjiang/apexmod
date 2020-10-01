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
      AddInclude(o => o.OrderItems);
      AddInclude(o => o.DeliveryMethod);
      AddOrderByDescending(o => o.OrderDate);
    }

    public OrdersWithItemsAndOrderingSpecification(int id, string email)
        : base(o => o.Id == id && o.BuyerEmail == email)
    {
      AddInclude(o => o.OrderItems);
      AddInclude(o => o.DeliveryMethod);
    }

    public OrdersWithItemsAndOrderingSpecification(int id)
        : base(o => o.Id == id)
    {
      AddInclude(o => o.OrderItems);
      AddInclude(o => o.DeliveryMethod);
    }

    public OrdersWithItemsAndOrderingSpecification()
    {
      AddInclude(o => o.OrderItems);
      AddInclude(o => o.DeliveryMethod);
      AddOrderByDescending(o => o.OrderDate);
    }
  }
}