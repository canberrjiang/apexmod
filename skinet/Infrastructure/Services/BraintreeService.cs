using System.Threading.Tasks;
using Braintree;
using Core.Entities.Braintree;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services
{
  public class BraintreeService : IBraintreeService
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _config;
    public BraintreeService(IUnitOfWork unitOfWork, IConfiguration config)
    {
      _config = config;
      _unitOfWork = unitOfWork;
    }

    public BraintreeGateway CreateGateway()
    {
      var gateway = new BraintreeGateway
      {
        Environment = Braintree.Environment.SANDBOX,
        MerchantId = _config["BraintreeMerchantId"],
        PublicKey = _config["BraintreePublicKey"],
        PrivateKey = _config["BraintreePrivateKey"]
      };
      return gateway;
    }

    public async Task<TransactionRequest> CreateBraintreeOrder(BraintreePurchaseRequest braintreePurchaseRequest)
    {
      var orderId = braintreePurchaseRequest.OrderId;
      var paymentMethodNonce = braintreePurchaseRequest.Nonce;
      var orderSpec = new Core.Specifications.OrderWithDeliveryMethodSpecification(orderId);
      var order = await _unitOfWork.Repository<Core.Entities.OrderAggregate.Order>().GetEntityWithSpec(orderSpec);
      var deliveryMethodPrice = (order != null) ? order.DeliveryMethod.Price : 0;
      var totalPrice = order.Subtotal + deliveryMethodPrice;
      // Only charge for deposit price if a user choses "Deposity Only".
      if (order.DeliveryMethod.ShortName == "Deposit Only")
      {
        totalPrice = order.DeliveryMethod.Price;
      }

      if (!string.IsNullOrEmpty(paymentMethodNonce))
      {
        var request = new TransactionRequest
        {
          Amount = totalPrice,
          PaymentMethodNonce = paymentMethodNonce,
          OrderId = orderId.ToString(),
          ShippingAddress = new AddressRequest()
          {
            FirstName = order.ShipToAddress.FirstName,
            LastName = order.ShipToAddress.LastName,
            StreetAddress = order.ShipToAddress.Street,
            Locality = order.ShipToAddress.City,
            Region = order.ShipToAddress.State,
            PostalCode = order.ShipToAddress.Zipcode
          },
          Options = new TransactionOptionsRequest
          {
            SubmitForSettlement = true
          }
        };

        return request;
      }

      return null;
    }

    public async Task<Core.Entities.OrderAggregate.Order> UpdateOrderPaymentFailed(int orderId)
    {
      var order = await _unitOfWork.Repository<Core.Entities.OrderAggregate.Order>().GetByIdAsync(orderId);
      if (order == null) return null;
      order.Status = OrderStatus.PaymentFailed;
      await _unitOfWork.Complete();
      return order;
    }

    public async Task<Core.Entities.OrderAggregate.Order> UpdateOrderPaymentSucceeded(int orderId)
    {
      var order = await _unitOfWork.Repository<Core.Entities.OrderAggregate.Order>().GetByIdAsync(orderId);
      if (order == null) return null;
      order.Status = OrderStatus.PaymentRecevied;
      await _unitOfWork.Complete();
      return order;
    }
  }
}