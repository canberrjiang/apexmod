using System;
using System.Linq;
using System.Linq.Expressions;
using Core.Entities;

namespace Core.Specifications
{
  public class ProductWithTagsAndCategoriesSpecification : BaseSpecification<Product>
  {
    public class ProductsWithTagAndCategorySpecification : BaseSpecification<Product>
    {
      public ProductsWithTagAndCategorySpecification(BaseProductsSpecParams productParams)
          : base(x =>
              (string.IsNullOrEmpty(productParams.Search) || x.Name.ToLower().Contains(productParams.Search)) &&
              (!productParams.ProductTagId.HasValue || x.ProductTag.Where(pt => pt.TagId == productParams.ProductTagId).Count() > 0)
          )
      {
        AddInclude(x => x.ProductCategory);
        AddInclude($"{nameof(Product.ProductTag)}.{nameof(ProductTag.Tag)}");
        AddInclude(x => x.ProductTag);
        AddInclude(x => x.ChildProducts);
        AddInclude($"{nameof(Product.ChildProducts)}.{nameof(ProductProduct.ChildProduct)}");
        AddInclude(x => x.Photos);
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

      public ProductsWithTagAndCategorySpecification(int id)
          : base(x => x.Id == id)
      {
        AddInclude(x => x.ProductCategory);
        AddInclude($"{nameof(Product.ProductTag)}.{nameof(ProductTag.Tag)}");
        AddInclude(x => x.ProductTag);
        AddInclude(x => x.ChildProducts);
        // AddInclude($"{nameof(Product.ProductCategory)}.{nameof(ProductCategory)}");
        AddInclude($"{nameof(Product.ChildProducts)}.{nameof(ProductProduct.ChildProduct)}");
        AddInclude($"{nameof(Product.ChildProducts)}.{nameof(ProductProduct.ChildProduct)}.{nameof(ChildProduct.ProductCategory)}");
        AddInclude(x => x.Photos);
      }
    }
  }
}