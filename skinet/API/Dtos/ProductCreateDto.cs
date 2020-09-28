using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Dtos
{
  public class ProductCreateDto
  {
    [Required]
    public string Name { get; set; }
    [Required]
    public string Discriminator { get; set; }
    public string Information { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    [RegularExpression(@"^\$?([0-9]{1,3},([0-9]{3},)*[0-9]{3}|[0-9]+)(.[0-9][0-9])?$",
            ErrorMessage = "Price must be a decimal (e.g 20.30)")]
    public decimal Price { get; set; }
    public string PictureUrl { get; set; }
    public List<int> ProductTagIds { get; set; }
    public int ProductCategoryId { get; set; }
    public List<ChildProductToCreate> SelectedChildProducts { get; set; }
    public bool IsPublished { get; set; }
  }
}