using System;
using System.Linq.Expressions;
using Core.Entities;

namespace Core.Specifications
{
  public class ProductByPublishStatusSpecification : BaseSpecification<BaseProduct>
  {
    public ProductByPublishStatusSpecification(bool isPublished) : base(x => (x.IsPublished == isPublished))
    {
    }
  }
}