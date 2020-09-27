using Core.Entities;

namespace Core.Specifications
{
  public class ProductTagWithProductAndTagSpecification : BaseSpecification<ProductTag>
  {
    public ProductTagWithProductAndTagSpecification() : base()
    {
      AddInclude(pt => pt.BaseProduct);
      AddInclude(pt => pt.Tag);
    }
  }
}