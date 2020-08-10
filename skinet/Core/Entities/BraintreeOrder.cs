namespace Core.Entities
{
  public class BraintreeOrder
  {
    public decimal Price { get; set; }
    public string PaymentMethodNonce { get; set; }
  }
}