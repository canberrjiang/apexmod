using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Dtos
{
  public class ProductToReturnDto
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string PictureUrl { get; set; }
    public string ProductGraphic { get; set; }
    public string ProductPlatform { get; set; }
    public IEnumerable<PhotoToReturnDto> Photos { get; set; }
    public IEnumerable<ProductComponentToReturnDto> ProductComponents { get; set; }
  }
}