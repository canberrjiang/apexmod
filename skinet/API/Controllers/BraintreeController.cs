using Braintree;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace API.Controllers
{
  public class BraintreeController : BaseApiController
  {
    private readonly IBraintreeConfiguration _braintreeConfiguration;
    public BraintreeController(IBraintreeConfiguration braintreeConfiguration)
    {
      _braintreeConfiguration = braintreeConfiguration;
    }

    [HttpGet]
    [Route("GenerateToken")]
    public object GenerateToken()
    {
      var gateway = _braintreeConfiguration.GetGateway();
      var clientToken = gateway.ClientToken.Generate();
      return clientToken;
    }

    [HttpPost]
    [Route("Checkout")]
    public object Checkout(BraintreeOrder order)
    {
      string paymentStatus = string.Empty;
      var gateway = _braintreeConfiguration.GetGateway();

      var request = new TransactionRequest
      {
        Amount = order.Price,
        PaymentMethodNonce = order.PaymentMethodNonce,
        Options = new TransactionOptionsRequest
        {
          SubmitForSettlement = true
        }
      };

      Result<Transaction> result = gateway.Transaction.Sale(request);
      if (result.IsSuccess())
      {

      }
      else
      {
        string errorMessages = "";
        foreach (ValidationError error in result.Errors.DeepAll())
        {
          errorMessages += "Error: " + (int)error.Code + " - " + error.Message + "\n";
        }

        paymentStatus = errorMessages;
      }
      return paymentStatus;
    }
  }
}