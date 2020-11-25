using PayPalCheckoutSdk.Core;

namespace Core.Interfaces
{
  public interface IPayPalService
  {
    PayPalHttpClient client();
  }
}