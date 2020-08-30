using Core.Entities.OrderAggregate;

namespace Core.Entities
{
  public class Photo : BasePhoto
  {
    public bool IsMain { get; set; }
    public BaseProduct Product { get; set; }
  }
}