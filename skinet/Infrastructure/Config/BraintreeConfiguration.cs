using Braintree;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Config
{
  public class BraintreeConfiguration : IBraintreeConfiguration
  {
    private readonly IConfiguration _config;
    public BraintreeConfiguration(IConfiguration config)
    {
      _config = config;
    }

    public string Environment { get; set; }
    public string MerchantId { get; set; }
    public string PublicKey { get; set; }
    public string PrivateKey { get; set; }
    private IBraintreeGateway BraintreeGateway { get; set; }
    public IBraintreeGateway CreateGateway()
    {
      MerchantId = _config["BraintreeMerchantId"];
      PublicKey = _config["BraintreePublicKey"];
      PrivateKey = _config["BraintreePrivateKey"];

      return new BraintreeGateway(Braintree.Environment.SANDBOX, MerchantId, PublicKey, PrivateKey);
    }

    public IBraintreeGateway GetGateway()
    {
      if (BraintreeGateway == null)
      {
        BraintreeGateway = CreateGateway();
      }

      return BraintreeGateway;
    }
  }
}