using System.Collections.Generic;

namespace Core.Entities
{
  public class BasketItem
  {
    public int Id { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string PictureUrl { get; set; }
    public List<Dictionary<string, int>> ChildProducts { get; set; }
  }
}