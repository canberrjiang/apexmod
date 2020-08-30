using System;
using System.Linq.Expressions;
using Core.Entities;

namespace Core.Specifications
{
  public class ProductTagByProductIdSpecification : BaseSpecification<ProductTag>
  {
    public ProductTagByProductIdSpecification(int productId)
        : base(pt =>
              (pt.ProductId == productId)
        )
    {
    }
  }
}