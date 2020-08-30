using System;
using System.Linq.Expressions;
using Core.Entities;

namespace Core.Specifications
{
  public class ProductProductByProductIdSpecification : BaseSpecification<ProductProduct>
  {
    public ProductProductByProductIdSpecification(int productId)
        : base(pp =>
        (pp.ProductId == productId))
    {
    }
  }
}