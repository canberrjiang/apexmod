using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Services
{
  public class ProductComponentService : IProductComponent
  {
    public ProductComponent CreateProductComponent(ProductComponent component)
    {
      var productComponent = new ProductComponent();
      productComponent.Title = component.Title;
      productComponent.Description = component.Description;
      productComponent.PPrice = component.PPrice;
      productComponent.TPrice = component.TPrice;
      return productComponent;
    }
  }
}