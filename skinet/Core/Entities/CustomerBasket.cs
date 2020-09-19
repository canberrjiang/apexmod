using System.Collections.Generic;

namespace Core.Entities
{
  public class CustomerBasket
  {
    // For redis only - avoid issues creating instance in Redis
    public CustomerBasket()
    {
    }

    public CustomerBasket(string id)
    {
      Id = id;

    }
    public string Id { get; set; }
    public List<BasketItem> Items { get; set; } = new List<BasketItem>();
    public int? DeliveryMethodId { get; set; }
    public decimal ShippingPrice { get; set; }
  }
}