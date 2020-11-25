using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using PayPalCheckoutSdk.Core;

namespace Infrastructure.Services
{
  public class PayPalService : IPayPalService
  {
    private readonly IConfiguration _config;

    public PayPalService(IConfiguration config)
    {
      _config = config;
    }

    public PayPalHttpClient client()
    {
      // Creating a sandbox environment
      PayPalEnvironment environment = new SandboxEnvironment(_config["Paypal:ClientId"], _config["Paypal:ClientSecret"]);

      // Create a production environment
      PayPalEnvironment prodEnvironment = new LiveEnvironment(_config["Paypal:LiveClientId"], _config["Paypal:LiveClientSecret"]);

      // Creating a client for the environment
      PayPalHttpClient client = new PayPalHttpClient(prodEnvironment);
      return client;
    }
  }
}