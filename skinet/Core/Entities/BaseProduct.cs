using System.Collections.Generic;
using System.Linq;

namespace Core.Entities
{
  public class BaseProduct : BaseEntity
  {
    public string Name { get; set; }
    public string Discriminator { get; set; }
    public string Information { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string PictureUrl { get; set; }
    public int Stock { get; set; }
    public List<ProductTag> ProductTag { get; set; }
    public int ProductCategoryId { get; set; }
    public ProductCategory ProductCategory { get; set; }
    private readonly List<Photo> _photos = new List<Photo>();
    public IReadOnlyList<Photo> Photos => _photos.AsReadOnly();
    public bool IsPublished { get; set; }

    public void AddPhoto(string pictureUrl, string fileName, bool isMain = false)
    {
      var photo = new Photo
      {
        FileName = fileName,
        PictureUrl = pictureUrl
      };

      if (_photos.Count == 0) photo.IsMain = true;

      _photos.Add(photo);
    }

    public void RemovePhoto(int id)
    {
      var photo = _photos.Find(x => x.Id == id);
      _photos.Remove(photo);
    }

    public void SetMainPhoto(int id)
    {

      var currentMain = _photos.SingleOrDefault(item => item.IsMain);
      foreach (var item in _photos.Where(item => item.IsMain))
      {
        item.IsMain = false;
      }

      var photo = _photos.Find(x => x.Id == id);

      if (photo != null)
      {
        photo.IsMain = true;
        if (currentMain != null) currentMain.IsMain = false;
      }
    }
  }
}