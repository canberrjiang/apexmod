using System;
using System.Linq.Expressions;
using Core.Entities;

namespace Core.Specifications
{
  public class AllBaseProductWithDiscriminatorAndCategory : BaseSpecification<BaseProduct>
  {
    public AllBaseProductWithDiscriminatorAndCategory(string discriminator) : base(
      x =>
      (x.Discriminator.ToLower() == discriminator)
      )
    {
      AddInclude(x => x.ProductCategory);
    }

    public AllBaseProductWithDiscriminatorAndCategory(int productCategoryId) : base(
      x =>
      (x.ProductCategoryId == productCategoryId)
      )
    {
      AddInclude(x => x.ProductCategory);
      AddInclude(x => x.Photos);
    }
  }
}