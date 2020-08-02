using Core.Entities.OrderAggregate;

namespace Core.Entities
{
  public class ProductComponent : BaseEntity
  {
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal PPrice { get; set; }
    public decimal TPrice { get; set; }
    public Product Product { get; set; }
    public int ProductId { get; set; }
  }
}