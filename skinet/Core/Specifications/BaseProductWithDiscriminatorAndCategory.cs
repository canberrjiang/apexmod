using System;
using System.Linq.Expressions;
using Core.Entities;

namespace Core.Specifications
{
  public class BaseProductWithDiscriminatorAndCategory : BaseSpecification<BaseProduct>
  {
    public BaseProductWithDiscriminatorAndCategory(string discriminator) : base(
      x =>
      (x.Discriminator.ToLower() == discriminator) &&
      (x.IsPublished == true)
      )
    {
      AddInclude(x => x.ProductCategory);
    }

    public BaseProductWithDiscriminatorAndCategory(int productCategoryId) : base(
      x =>
      (x.ProductCategoryId == productCategoryId) &&
      (x.IsPublished == true))
    {
      AddInclude(x => x.ProductCategory);
    }
  }
}