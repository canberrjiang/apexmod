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
    public ComponentPhoto Photo { get; set; }

    public ComponentPhoto AddComponentPhoto(string pictureUrl, string fileName)
    {
      var photo = new ComponentPhoto
      {
        FileName = fileName,
        PictureUrl = pictureUrl
      };
      this.Photo = photo;
      return photo;
    }

    public void RemoveComponentPhoto(int id)
    {
      this.Photo = null;
    }
  }
}