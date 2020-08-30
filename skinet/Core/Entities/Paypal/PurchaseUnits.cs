namespace Core.Entities.Paypal
{
  public class PurchaseUnits
  {
    public string reference_id { get; set; }
    public PaypalAmount amount { get; set; }
  }
}