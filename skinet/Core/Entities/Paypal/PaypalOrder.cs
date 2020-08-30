using System.Collections.Generic;

namespace Core.Entities.Paypal
{
  public class PaypalOrder
  {
    public string intent { get; set; }
    public List<PurchaseUnits> purchase_units { get; set; }
  }
}