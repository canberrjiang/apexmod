using System.Collections.Generic;
using System.Linq;

namespace Core.Entities
{
  public class Product : BaseProduct
  {
    public virtual List<ProductProduct> ChildProducts { get; set; }
  }
}