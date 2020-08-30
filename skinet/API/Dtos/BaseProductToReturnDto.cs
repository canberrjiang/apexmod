using System.Collections.Generic;

namespace API.Dtos
{
  public class BaseProductToReturnDto
  {
    public int Id { get; set; }
    public string Discriminator { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string PictureUrl { get; set; }
    public string ProductCategory { get; set; }
    public IEnumerable<TagToReturnDto> Tags { get; set; }
    public IEnumerable<PhotoToReturnDto> Photos { get; set; }
    public bool IsPublished { get; set; }
  }
}