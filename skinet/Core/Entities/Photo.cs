using Core.Entities.OrderAggregate;

namespace Core.Entities
{
  public class Photo : BasePhoto
  {
    public bool IsMain { get; set; }
    public Product Product { get; set; }
    public int ProductId { get; set; }
  }
}