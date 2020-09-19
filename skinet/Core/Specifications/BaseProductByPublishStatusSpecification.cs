using System;
using System.Linq.Expressions;
using Core.Entities;

namespace Core.Specifications
{
  public class BaseProductByPublishStatusSpecification : BaseSpecification<BaseProduct>
  {
    public BaseProductByPublishStatusSpecification(bool isPublished) : base(
      x => (x.IsPublished == isPublished))
    {
    }
  }
}