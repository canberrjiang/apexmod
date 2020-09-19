using System;
using System.Threading.Tasks;
using Braintree;
using Core.Entities.Braintree;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
  public class BraintreeController : BaseApiController
  {
    private readonly IBraintreeService _braintreeService;
    private readonly IEmailService _emailService;
    public BraintreeController(IBraintreeService braintreeService, IEmailService emailService)
    {
      _emailService = emailService;
      _braintreeService = braintreeService;
    }

    [HttpGet]
    public ActionResult<string> GenerateClientToken()
    {
      var gateway = _braintreeService.CreateGateway();
      var clientToken = gateway.ClientToken.Generate();

      return Ok(clientToken);
    }

    [HttpPost]
    public async Task<ActionResult> CreatePurchase(BraintreePurchaseRequest order)
    {
      var gateway = _braintreeService.CreateGateway();
      string nonceFromTheClient = order.Nonce;

      if (!string.IsNullOrEmpty(nonceFromTheClient))
      {
        var request = await _braintreeService.CreateBraintreeOrder(order);
        if (request == null) return BadRequest("Failed to process payment");
        Result<Transaction> result = gateway.Transaction.Sale(request);
        if (result.IsSuccess())
        {
          await _braintreeService.UpdateOrderPaymentSucceeded(order.OrderId);
          Transaction transaction = result.Target;
          Console.WriteLine("Success!: " + transaction.Id);
        }
        else if (result.Transaction != null)
        {
          await _braintreeService.UpdateOrderPaymentFailed(order.OrderId);
          Transaction transaction = result.Transaction;
          Console.WriteLine("Error processing transaction:");
          Console.WriteLine("  Status: " + transaction.Status);
          Console.WriteLine("  Code: " + transaction.ProcessorResponseCode);
          Console.WriteLine("  Text: " + transaction.ProcessorResponseText);
        }
        else
        {
          foreach (ValidationError error in result.Errors.DeepAll())
          {
            Console.WriteLine("Attribute: " + error.Attribute);
            Console.WriteLine("  Code: " + error.Code);
            Console.WriteLine("  Message: " + error.Message);
          }
        }
      }

      return Ok();
    }

    [HttpGet("email")]
    public ActionResult SendEmail()
    {
      _emailService.SendEmail();
      return Ok();
    }
  }
}