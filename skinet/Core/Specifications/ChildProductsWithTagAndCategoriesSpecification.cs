using Core.Entities;

namespace Core.Specifications
{
  public class ChildProductsWithTagAndCategoriesSpecification : BaseSpecification<ChildProduct>
  {
    public ChildProductsWithTagAndCategoriesSpecification(int id)
          : base(x => x.Id == id)
    {
      AddInclude(x => x.ProductCategory);
      AddInclude(x => x.ProductTag);
      AddInclude($"{nameof(ChildProduct.ProductTag)}.{nameof(ProductTag.Tag)}");
      AddInclude($"{nameof(ChildProduct.Photos)}");
    }
  }
}