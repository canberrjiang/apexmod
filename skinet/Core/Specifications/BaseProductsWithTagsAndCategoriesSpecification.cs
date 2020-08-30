using System;
using System.Linq;
using System.Linq.Expressions;
using Core.Entities;

namespace Core.Specifications
{
  public class BaseProductWithTagsAndCategoriesSpecification : BaseSpecification<BaseProduct>
  {
    public class BaseProductsWithTagsAndCategoriesSpecification : BaseSpecification<BaseProduct>
    {
      public BaseProductsWithTagsAndCategoriesSpecification(BaseProductsSpecParams baseProductParams)
          : base(x =>
              (string.IsNullOrEmpty(baseProductParams.Search) || x.Name.ToLower().Contains(baseProductParams.Search)) &&
              (!baseProductParams.ProductCategoryId.HasValue || x.ProductCategoryId == baseProductParams.ProductCategoryId)
          )
      {
        AddInclude(x => x.ProductCategory);
        AddInclude(x => x.ProductTag);
        AddInclude($"{nameof(BaseProduct.ProductTag)}.{nameof(ProductTag.Tag)}");
        AddInclude($"{nameof(BaseProduct.Photos)}");
        AddOrderBy(x => x.Name);
        ApplyPaging(baseProductParams.PageSize * (baseProductParams.PageIndex - 1), baseProductParams.PageSize);

        if (!string.IsNullOrEmpty(baseProductParams.Sort))
        {
          switch (baseProductParams.Sort)
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

      public BaseProductsWithTagsAndCategoriesSpecification(int id)
          : base(x => x.Id == id)
      {
        AddInclude(x => x.ProductCategory);
        AddInclude(x => x.ProductTag);
        AddInclude($"{nameof(BaseProduct.ProductTag)}.{nameof(ProductTag.Tag)}");
        AddInclude($"{nameof(BaseProduct.Photos)}");
      }
    }
  }
}