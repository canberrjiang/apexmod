using System;
using System.Linq;
using System.Linq.Expressions;
using Core.Entities;

namespace Core.Specifications
{
  public class AllBaseProductWithFiltersForCountSpecification : BaseSpecification<BaseProduct>
  {
    public AllBaseProductWithFiltersForCountSpecification(BaseProductsSpecParams baseProductParams)
      : base(x =>
            (string.IsNullOrEmpty(baseProductParams.Search) || x.Name.ToLower().Contains(baseProductParams.Search)) &&
          (!baseProductParams.ProductTagId.HasValue || x.ProductTag.Where(pt => pt.TagId == baseProductParams.ProductTagId).Count() > 0
          )
      )
    {
    }
  }
}