
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Mvc;
using PayPalCheckoutSdk.Orders;
using PayPalHttp;

namespace API.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class PaypalController : BaseApiController
  {
    private readonly IPayPalService _paypalService;
    private readonly IUnitOfWork _unitOfWork;
    public PaypalController(IPayPalService paypalService, IUnitOfWork unitOfWork)
    {
      _unitOfWork = unitOfWork;
      _paypalService = paypalService;
    }

    [HttpPost("{id}")]
    public async Task<Order> CreatePayPalOrder(int id)
    {
      var order = new OrderRequest();
      var orderSpec = new OrderWithDeliveryMethodSpecification(id);
      var myOrder = await _unitOfWork.Repository<Core.Entities.OrderAggregate.Order>().GetEntityWithSpec(orderSpec);
      var myOrderSubtotal = myOrder.Subtotal;
      var shippingCost = myOrder.Subtotal >= 1000 ? 0 : myOrder.DeliveryMethod.Price;
      HttpResponse response;
      if (myOrder.Status == Core.Entities.OrderAggregate.OrderStatus.PaymentRecevied) return new Order();
      if (myOrder.DeliveryMethod.Id == 4)
      {
        myOrderSubtotal = myOrder.DeliveryMethod.Price;
        order = new OrderRequest()
        {
          CheckoutPaymentIntent = "CAPTURE",
          PurchaseUnits = new List<PurchaseUnitRequest>()
                {
                    new PurchaseUnitRequest()
                    {
                        AmountWithBreakdown = new AmountWithBreakdown()
                        {
                            CurrencyCode = "AUD",
                            Value = myOrder.GetTotal().ToString()
                        },
                        ShippingDetail = new ShippingDetail()
                        {
                          AddressPortable = new AddressPortable()
                          {
                            AddressLine1 = myOrder.ShipToAddress.Street,
                            AdminArea1 = myOrder.ShipToAddress.City,
                            AdminArea2 = myOrder.ShipToAddress.State,
                            PostalCode = myOrder.ShipToAddress.Zipcode,
                            CountryCode = "AU"
                          },
                          Name = new Name()
                          {
                            FullName = myOrder.ShipToAddress.FirstName + " " + myOrder.ShipToAddress.LastName
                          }
                        }
                    }
                },
          ApplicationContext = new ApplicationContext()
          {
            ReturnUrl = "https://toplayer.com.au/",
            CancelUrl = "https://toplayer.com.au/"
          }
        };
      }
      else
      {
        // Construct a request object and set desired parameters
        // Create a POST request to /v2/checkout/orders
        order = new OrderRequest()
        {
          CheckoutPaymentIntent = "CAPTURE",
          PurchaseUnits = new List<PurchaseUnitRequest>()
                {
                    new PurchaseUnitRequest()
                    {
                        AmountWithBreakdown = new AmountWithBreakdown()
                        {
                            CurrencyCode = "AUD",
                            AmountBreakdown = new AmountBreakdown()
                            {
                              ItemTotal = new Money()
                              {
                                CurrencyCode = "AUD",
                                Value = myOrder.Subtotal.ToString()
                              },
                              Shipping = new Money()
                              {
                                CurrencyCode = "AUD",
                                Value = shippingCost.ToString()
                              }
                            },
                            Value = myOrder.GetTotal().ToString()
                        },
                        ShippingDetail = new ShippingDetail()
                        {
                          AddressPortable = new AddressPortable()
                          {
                            AddressLine1 = myOrder.ShipToAddress.Street,
                            AdminArea1 = myOrder.ShipToAddress.City,
                            AdminArea2 = myOrder.ShipToAddress.State,
                            PostalCode = myOrder.ShipToAddress.Zipcode,
                            CountryCode = "AU"
                          },
                          Name = new Name()
                          {
                            FullName = myOrder.ShipToAddress.FirstName + " " + myOrder.ShipToAddress.LastName
                          }
                        }
                    }
                },
          ApplicationContext = new ApplicationContext()
          {
            ReturnUrl = "https://apexmod.com.au/",
            CancelUrl = "https://apexmod.com.au/"
          }
        };

      }

      // Call Paypal API
      var request = new OrdersCreateRequest();
      request.Prefer("return=representation");
      request.RequestBody(order);

      response = await _paypalService.client().Execute(request);
      var statusCode = response.StatusCode;
      Order result = response.Result<Order>();
      return result;
    }

    [HttpPost("capture/{id}/{orderId}")]
    public async Task<Order> CapturePayPalOrder(string id, int orderId)
    {
      // Construct a request object and set desired parameters
      var order = await _unitOfWork.Repository<Core.Entities.OrderAggregate.Order>().GetByIdAsync(orderId);
      var request = new OrdersCaptureRequest(id);
      request.RequestBody(new OrderActionRequest());
      HttpResponse response = await _paypalService.client().Execute(request);
      Order result = new Order();

      // Update my order status
      if (response.StatusCode != System.Net.HttpStatusCode.Created)
      {
        order.Status = Core.Entities.OrderAggregate.OrderStatus.PaymentFailed;
      }
      else
      {
        order.Status = Core.Entities.OrderAggregate.OrderStatus.PaymentRecevied;
      }
      await _unitOfWork.Complete();

      result = response.Result<Order>();
      return result;
    }
  }
}