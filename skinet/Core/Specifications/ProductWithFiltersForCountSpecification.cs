using System;
using System.Linq.Expressions;
using Core.Entities;

namespace Core.Specifications
{
  public class ProductWithFiltersForCountSpecification : BaseSpecification<Product>
  {
    public ProductWithFiltersForCountSpecification(BaseProductsSpecParams productParams)
      : base(x =>
            (string.IsNullOrEmpty(productParams.Search) || x.Name.ToLower().Contains(productParams.Search)) &&
          (!productParams.ProductCategoryId.HasValue || x.ProductCategoryId == productParams.ProductCategoryId)
      )
    {
    }
  }
}