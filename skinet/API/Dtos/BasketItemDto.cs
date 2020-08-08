using System.ComponentModel.DataAnnotations;

namespace API.Dtos
{
  public class BasketItemDto
  {
    [Required]
    public int Id { get; set; }

    [Required]
    public string ProductName { get; set; }

    // Todo - update the minimun value
    [Required]
    [Range(0.1, double.MaxValue, ErrorMessage = "Price msut be greater than zero")]
    public decimal Price { get; set; }

    [Required]
    [Range(1, double.MaxValue, ErrorMessage = "Quantity msut be at least one")]
    public int Quantity { get; set; }

    [Required]
    public string PictureUrl { get; set; }

    [Required]
    public string Graphic { get; set; }

    [Required]
    public string Platform { get; set; }
  }
}