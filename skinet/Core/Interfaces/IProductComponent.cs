using System.Threading.Tasks;
using Core.Entities;

namespace Core.Interfaces
{
  public interface IProductComponent
  {
    ProductComponent CreateProductComponent(ProductComponent component);
  }
}