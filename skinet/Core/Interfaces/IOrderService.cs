using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities.OrderAggregate;

namespace Core.Interfaces
{
  public interface IOrderService
  {
    Task<Order> CreateOrderAsync(string buyerEmail, int deliveryMethodId, string basketId, Address shippingAddress);
    Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail);
    Task<Order> GetOrderByIdAsync(int id, string buyerEmail);
    Task<Order> GetOrderByIdAsync(int id);
    Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync();
    Task<DeliveryMethod> GetDeliveryMethodByIdAsync(int id);
    Task<IReadOnlyList<Order>> GetAllOrders();
    Task<Order> DeleteOrder(Order order);
  }
}