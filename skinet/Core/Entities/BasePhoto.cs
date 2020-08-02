using Core.Entities.OrderAggregate;

namespace Core.Entities
{
  public class BasePhoto : BaseEntity
  {
    public string PictureUrl { get; set; }
    public string FileName { get; set; }
  }
}