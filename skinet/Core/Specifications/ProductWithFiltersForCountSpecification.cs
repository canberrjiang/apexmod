using System;
using System.Linq.Expressions;
using Core.Entities.OrderAggregate;

namespace Core.Specifications
{
  public class ProductWithFiltersForCountSpecification : BaseSpecification<Product>
  {
    public ProductWithFiltersForCountSpecification(ProductsSpecParams productParams)
      : base(x =>
            (string.IsNullOrEmpty(productParams.Search) || x.Name.ToLower().Contains(productParams.Search)) &&
          (!productParams.GraphicId.HasValue || x.ProductGraphicId == productParams.GraphicId) &&
          (!productParams.PlatformId.HasValue || x.ProductPlatformId == productParams.PlatformId)
      )
    {
    }
  }
}