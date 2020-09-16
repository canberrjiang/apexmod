namespace Core.Entities.Braintree
{
  public class BraintreePurchaseRequest
  {
    public int OrderId { get; set; }
    public string Nonce { get; set; }
  }
}