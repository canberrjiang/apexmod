using System;
using System.Linq.Expressions;
using Core.Entities.OrderAggregate;

namespace Core.Specifications
{
  public class ProductWithPlatformsAndGraphicsSpecification : BaseSpecification<Product>
  {
    public class ProductsWithPlatformsAndGraphicsSpecification : BaseSpecification<Product>
    {
      public ProductsWithPlatformsAndGraphicsSpecification(ProductsSpecParams productParams)
          : base(x =>
              (string.IsNullOrEmpty(productParams.Search) || x.Name.ToLower().Contains(productParams.Search)) &&
              (!productParams.GraphicId.HasValue || x.ProductGraphicId == productParams.GraphicId) &&
              (!productParams.PlatformId.HasValue || x.ProductPlatformId == productParams.PlatformId)
          )
      {
        AddInclude(x => x.ProductGraphic);
        AddInclude(x => x.ProductPlatform);
        AddInclude(x => x.Photos);
        AddInclude(x => x.ProductComponents);
        AddOrderBy(x => x.Name);
        ApplyPaging(productParams.PageSize * (productParams.PageIndex - 1), productParams.PageSize);

        if (!string.IsNullOrEmpty(productParams.Sort))
        {
          switch (productParams.Sort)
          {
            case "priceAsc":
              AddOrderBy(p => p.Price);
              break;
            case "priceDesc":
              AddOrderByDescending(p => p.Price);
              break;
            default:
              AddOrderBy(n => n.Name);
              break;
          }
        }
      }

      public ProductsWithPlatformsAndGraphicsSpecification(int id)
          : base(x => x.Id == id)
      {
        AddInclude(x => x.ProductGraphic);
        AddInclude(x => x.ProductPlatform);
        AddInclude(x => x.Photos);
        AddInclude(x => x.ProductComponents);
      }
    }
  }
}