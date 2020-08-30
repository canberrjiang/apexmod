using System.Net.Http;
using System.Threading.Tasks;
using Core.Entities.Paypal;

namespace Core.Interfaces
{
  public interface IPaypalService
  {
    public PaypalToken GetAccessToken();
    public Task<PaypalOrderToReturn> CreateOrder(string token);
    public Task<string> AuthorizePayment(string token, string orderId);
  }
}