using System.Threading.Tasks;
using Braintree;
using Core.Entities.Braintree;

namespace Core.Interfaces
{
  public interface IBraintreeService
  {
    public BraintreeGateway CreateGateway();
    Task<TransactionRequest> CreateBraintreeOrder(BraintreePurchaseRequest braintreePurchaseRequest);
    Task<Core.Entities.OrderAggregate.Order> UpdateOrderPaymentFailed(int orderId);
    Task<Core.Entities.OrderAggregate.Order> UpdateOrderPaymentSucceeded(int orderId);
  }
}