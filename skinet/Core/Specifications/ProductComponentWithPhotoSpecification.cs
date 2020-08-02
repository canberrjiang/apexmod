using System;
using System.Linq.Expressions;
using Core.Entities;
using Core.Entities.OrderAggregate;

namespace Core.Specifications
{
  public class ProductComponentWithPhotoSpecification : BaseSpecification<ProductComponent>
  {
    public class ProductComponentsWithPhotoSpecification : BaseSpecification<ProductComponent>
    {
      public ProductComponentsWithPhotoSpecification()
      {
      }

      public ProductComponentsWithPhotoSpecification(int id) : base(x => x.Id == id)
      {
        AddInclude(x => x.Photo);
      }
    }
  }
}