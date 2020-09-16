using System.Threading.Tasks;
using Braintree;
using Core.Entities.Braintree;
using Core.Entities.OrderAggregate;
using Core.Interfaces;

namespace Infrastructure.Services
{
  public class BraintreeService : IBraintreeService
  {
    private readonly IUnitOfWork _unitOfWork;
    public BraintreeService(IUnitOfWork unitOfWork)
    {
      _unitOfWork = unitOfWork;
    }

    public BraintreeGateway CreateGateway()
    {
      var gateway = new BraintreeGateway
      {
        Environment = Braintree.Environment.SANDBOX,
        MerchantId = "c2xsphr95p2v96qn",
        PublicKey = "39nkzcvs9rz634pb",
        PrivateKey = "4c1f8685a34c8fd93a509e411b5b0f2d"
      };
      return gateway;
    }

    public async Task<TransactionRequest> CreateBraintreeOrder(BraintreePurchaseRequest braintreePurchaseRequest)
    {
      var orderId = braintreePurchaseRequest.OrderId;
      var paymentMethodNonce = braintreePurchaseRequest.Nonce;
      var order = await _unitOfWork.Repository<Core.Entities.OrderAggregate.Order>().GetByIdAsync(orderId);
      var deliveryMethodPrice = (order.DeliveryMethod != null) ? order.DeliveryMethod.Price : 0;
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