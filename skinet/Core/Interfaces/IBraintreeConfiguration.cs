using Braintree;

namespace Core.Interfaces
{
  public interface IBraintreeConfiguration
  {
    IBraintreeGateway CreateGateway();
    IBraintreeGateway GetGateway();
  }
}