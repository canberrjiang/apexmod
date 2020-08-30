using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Infrastructure.Config;
using Microsoft.AspNetCore.Mvc;
using PayPalCheckoutSdk.Orders;
using PayPalCheckoutSdk.Payments;
using PayPalHttp;
using Core.Interfaces;
using Newtonsoft.Json;
using System.Text;
using Core.Entities.Paypal;
using System.Net;

namespace API.Controllers
{
  public class PaypalController : BaseApiController
  {
    private readonly IHttpClientFactory _clientFactory;
    private readonly IPaypalService _paypalService;
    public PaypalController(IHttpClientFactory clientFactory, IPaypalService paypalService)
    {
      _paypalService = paypalService;
      _clientFactory = clientFactory;
    }

    //2. Set up your server to receive a call from the client
    /*
      Method to create order

      @param debug true = print response data
      @return HttpResponse<Order> response received from API
      @throws IOException Exceptions from API if any
    */

    [HttpGet]
    [Route("gettoken")]
    public string GetToken()
    {
      var paypalAccessToken = _paypalService.GetAccessToken();
      var token = paypalAccessToken.Access_Token;
      return token;
    }

    [HttpGet]
    [Route("createorder")]
    public async Task<PaypalOrderToReturn> CreateOrder()
    {
      var paypalAccessToken = _paypalService.GetAccessToken();
      var token = paypalAccessToken.Access_Token;
      var response = await _paypalService.CreateOrder(token);
      if (response.status == "CREATED") return response;
      return null;
    }

    // [HttpPost]
    // [Route("authorizepayment/{token}/{id}")]
    // public async Task<string> AuthorizePayment(string token, string id)
    // {
    //   var res = await _paypalService.AuthorizePayment(token, id);
    //   return res;
    // }

    // Retrieve an order from Paypal.

    // [HttpGet]
    // [Route("getorder/{id}")]
    // public async Task<HttpResponse> GetOrder(string id)
    // {
    //     OrdersGetRequest request = new OrdersGetRequest(id);
    //     // Call Paypal to get the transaction
    //     var response = await PaypalConfig.client().Execute(request);
    //     // Save the transaction in your database. Implement logic to save transaction to your database for future reference.
    // }


    // Method to capture order after creation. Pass a valid, approved order ID as an argument to build this method.
    public async Task<HttpResponse> CaptureOrder(string Id, bool debug = false)
    {
      var request = new OrdersCaptureRequest(Id);
      request.Prefer("return-representation");
      request.RequestBody(new OrderActionRequest());
      var response = await PaypalConfig.client().Execute(request);

      //     if (debug)
      //   {
      //     var result = response.Result<Order>();
      //     Console.WriteLine("Status: {0}", result.Status);
      //     Console.WriteLine("Order Id: {0}", result.Id);
      //     Console.WriteLine("Intent: {0}", result.Intent);
      //     Console.WriteLine("Links:");
      //     foreach (LinkDescription link in result.Links)
      //     {
      //       Console.WriteLine("\t{0}: {1}\tCall Type: {2}", link.Rel, link.Href, link.Method);
      //     }
      //     Console.WriteLine("Capture Ids: ");
      //     foreach (PurchaseUnit purchaseUnit in result.PurchaseUnits)
      //     {
      //       foreach (Capture capture in purchaseUnit.Payments.Captures)
      //       {
      //         Console.WriteLine("\t {0}", capture.Id);
      //       }
      //     }
      //     AmountWithBreakdown amount = result.PurchaseUnits[0].Amount;
      //     Console.WriteLine("Buyer:");
      //     Console.WriteLine("\tEmail Address: {0}\n\tName: {1}\n\tPhone Number: {2}{3}", result.Payer.EmailAddress, result.Payer.Name.FullName, result.Payer.Phone.CountryCode, result.Payer.Phone.NationalNumber);
      //   }
      return response;
    }

    // Use this function to perform authorization on the approved order.
    public async Task<HttpResponse> AuthorizeOrder(string Id, bool debug = false)
    {
      var request = new OrdersAuthorizeRequest(Id);
      request.Prefer("return=representation");
      request.RequestBody(new AuthorizeRequest());
      //3. Call PayPal to authorization an order
      var response = await PaypalConfig.client().Execute(request);
      //4. Save the authorization ID to your database. Implement logic to save the authorization to your database for future reference.
      //   if (debug)
      //   {
      //     var result = response.Result<Order>();
      //     Console.WriteLine("Status: {0}", result.Status);
      //     Console.WriteLine("Order Id: {0}", result.Id);
      //     Console.WriteLine("Authorization Id: {0}",
      //                     result.PurchaseUnits[0].Payments.Authorizations[0].Id);
      //     Console.WriteLine("Intent: {0}", result.Intent);
      //     Console.WriteLine("Links:");
      //     foreach (LinkDescription link in result.Links)
      //     {
      //         Console.WriteLine("\t{0}: {1}\tCall Type: {2}", link.Rel,
      //                                                         link.Href,
      //                                                         link.Method);
      //     }
      //     AmountWithBreakdown amount = result.PurchaseUnits[0].Amount;
      //     Console.WriteLine("Buyer:");
      //     Console.WriteLine("\tEmail Address: {0}", result.Payer.EmailAddress);
      //     Console.WriteLine("Response JSON: \n {0}",
      //                                 PayPalClient.ObjectToJSONString(result));
      //   }

      return response;
    }

    public async Task<HttpResponse> CaptureAuth(string Id, bool debug = false)
    {
      var request = new AuthorizationsCaptureRequest(Id);
      request.Prefer("return=representation");
      request.RequestBody(new CaptureRequest());
      //3. Call Paypal to capture an authorization.
      var response = await PaypalConfig.client().Execute(request);
      //4. Save the capture ID to your database for future reference.
      //   if (debug)
      //   {
      //     var result = response.Result<Capture>();
      //     Console.WriteLine("Status: {0}", result.Status);
      //     Console.WriteLine("Order Id: {0}", result.Id);
      //     Console.WriteLine("Links:");
      //     foreach (LinkDescription link in result.Links)
      //     {
      //       Console.WriteLine("\t{0}: {1}\tCall Type: {2}", link.Rel,
      //                               link.Href,
      //                               link.Method);
      //     }
      //     Console.WriteLine("Response JSON: \n {0}",
      //                 PayPalClient.ObjectToJSONString(result));
      //   }
      return response;
    }
    /*
      Method to generate sample create order body with AUTHORIZE intent

      @return OrderRequest with created order request
     */
    private static OrderRequest BuildRequestBody()
    {
      OrderRequest orderRequest = new OrderRequest()
      {
        CheckoutPaymentIntent = "AUTHORIZE",

        ApplicationContext = new ApplicationContext
        {
          BrandName = "EXAMPLE INC",
          LandingPage = "BILLING",
          UserAction = "CONTINUE",
          ShippingPreference = "SET_PROVIDED_ADDRESS"
        },
        PurchaseUnits = new List<PurchaseUnitRequest>
        {
          new PurchaseUnitRequest{
            ReferenceId =  "PUHF",
            Description = "Sporting Goods",
            CustomId = "CUST-HighFashions",
            SoftDescriptor = "HighFashions",
            AmountWithBreakdown = new AmountWithBreakdown
            {
              CurrencyCode = "USD",
              Value = "230.00",
              AmountBreakdown = new AmountBreakdown
              {
                ItemTotal = new PayPalCheckoutSdk.Orders.Money
                {
                  CurrencyCode = "USD",
                  Value = "180.00"
                },
                Shipping = new PayPalCheckoutSdk.Orders.Money
                {
                  CurrencyCode = "USD",
                  Value = "30.00"
                },
                Handling = new PayPalCheckoutSdk.Orders.Money
                {
                  CurrencyCode = "USD",
                  Value = "10.00"
                },
                TaxTotal = new PayPalCheckoutSdk.Orders.Money
                {
                  CurrencyCode = "USD",
                  Value = "20.00"
                },
                ShippingDiscount = new PayPalCheckoutSdk.Orders.Money
                {
                  CurrencyCode = "USD",
                  Value = "10.00"
                }
              }
            },
            Items = new List<Item>
            {
              new Item
              {
                Name = "T-shirt",
                Description = "Green XL",
                Sku = "sku01",
                UnitAmount = new PayPalCheckoutSdk.Orders.Money
                {
                  CurrencyCode = "USD",
                  Value = "90.00"
                },
                Tax = new PayPalCheckoutSdk.Orders.Money
                {
                  CurrencyCode = "USD",
                  Value = "10.00"
                },
                Quantity = "1",
                Category = "PHYSICAL_GOODS"
              },
              new Item
              {
                Name = "Shoes",
                Description = "Running, Size 10.5",
                Sku = "sku02",
                UnitAmount = new PayPalCheckoutSdk.Orders.Money
                {
                  CurrencyCode = "USD",
                  Value = "45.00"
                },
                Tax = new PayPalCheckoutSdk.Orders.Money
                {
                  CurrencyCode = "USD",
                  Value = "5.00"
                },
                Quantity = "2",
                Category = "PHYSICAL_GOODS"
              }
            },
            ShippingDetail = new ShippingDetail
            {
              Name = new Name
              {
                FullName = "John Doe"
              },
              AddressPortable = new AddressPortable
              {
                AddressLine1 = "123 Townsend St",
                AddressLine2 = "Floor 6",
                AdminArea2 = "San Francisco",
                AdminArea1 = "CA",
                PostalCode = "94107",
                CountryCode = "US"
              }
            }
          }
        }
      };

      return orderRequest;
    }
  }
}