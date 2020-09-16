using System;
using System.Linq.Expressions;
using Core.Entities.OrderAggregate;

namespace Core.Specifications
{
  public class OrderWithDeliveryMethodSpecification : BaseSpecification<Order>
  {
    public OrderWithDeliveryMethodSpecification(int orderId) : base(x => x.Id == orderId)
    {
      AddInclude(o => o.DeliveryMethod);
    }
  }
}