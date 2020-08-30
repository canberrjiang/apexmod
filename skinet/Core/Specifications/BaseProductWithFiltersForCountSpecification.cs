using System;
using System.Linq.Expressions;
using Core.Entities;

namespace Core.Specifications
{
  public class BaseProductWithFiltersForCountSpecification : BaseSpecification<BaseProduct>
  {
    public BaseProductWithFiltersForCountSpecification(BaseProductsSpecParams baseProductParams)
      : base(x =>
            (string.IsNullOrEmpty(baseProductParams.Search) || x.Name.ToLower().Contains(baseProductParams.Search)) &&
          (!baseProductParams.ProductCategoryId.HasValue || x.ProductCategoryId == baseProductParams.ProductCategoryId)
      )
    {
    }
  }
}