using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

using System.Threading.Tasks;
using Core.Entities.Paypal;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Infrastructure.Services
{
  public class PaypalService : IPaypalService
  {

    private readonly IConfiguration _config;
    public PaypalService(IConfiguration config)
    {
      _config = config;
    }

    public PaypalToken GetAccessToken()
    {
      var userName = _config["PaypalClient"];
      var password = _config["PaypalSecret"];
      var url = "https://api.sandbox.paypal.com/v1/oauth2/token";

      using (var client = new HttpClient())
      {
        var authToken = Encoding.ASCII.GetBytes($"{userName}:{password}");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
          "Basic", Convert.ToBase64String(authToken)
        );

        var dict = new Dictionary<string, string>();
        dict.Add("grant_type", "client_credentials");

        var response = client.PostAsync(url, new FormUrlEncodedContent(dict)).Result;
        var result = response.Content.ReadAsStringAsync().Result;
        var token = JsonConvert.DeserializeObject<PaypalToken>(result);
        return token;
      }
    }

    public async Task<PaypalOrderToReturn> CreateOrder(string token)
    {
      using (HttpClient client = new HttpClient())
      {
        var purchaseUnit = new PurchaseUnits()
        {
          reference_id = "PUHF",
          amount = new PaypalAmount()
          {
            currency_code = "AUD",
            value = "200.00"
          }
        };

        List<PurchaseUnits> purchaseUnits = new List<PurchaseUnits>();
        purchaseUnits.Add(purchaseUnit);
        var order = new PaypalOrder()
        {
          intent = "CAPTURE",
          purchase_units = purchaseUnits
        };

        var json = JsonConvert.SerializeObject(order);
        var data = new StringContent(json, Encoding.UTF8, "application/json");

        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
        var response = await client.PostAsync("https://api.sandbox.paypal.com/v2/checkout/orders", data);
        var res = response.Content.ReadAsStringAsync().Result;
        var orderReturned = JsonConvert.DeserializeObject<PaypalOrderToReturn>(res);
        return orderReturned;
      }
    }

    public async Task<string> AuthorizePayment(string token, string orderId)
    {
      using (HttpClient client = new HttpClient())
      {
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
        var response = await client.PostAsync($"https://api.sandbox.paypal.com/v2/checkout/orders/{orderId}/authorize", null);
        var res = response.Content.ReadAsStringAsync().Result;
        return res;
      }
    }
  }
}