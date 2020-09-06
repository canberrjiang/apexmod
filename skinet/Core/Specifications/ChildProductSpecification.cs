using System;
using System.Linq.Expressions;
using Core.Entities;

namespace Core.Specifications
{
  public class ChildProductSpecification : BaseSpecification<BaseProduct>
  {
    public ChildProductSpecification(string discriminator) : base(x => (x.Discriminator.ToLower() == discriminator))
    {
    }
  }
}