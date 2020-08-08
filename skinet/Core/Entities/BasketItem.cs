namespace Core.Entities.OrderAggregate
{
  public class BasketItem
  {
    public int Id { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string PictureUrl { get; set; }
    public string Graphic { get; set; }
    public string Platform { get; set; }
  }
}