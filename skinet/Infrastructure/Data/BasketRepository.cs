using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using StackExchange.Redis;

namespace Infrastructure.Data
{
  public class BasketRepository : IBasketRepository
  {
    private readonly IDatabase _database;
    private readonly IUnitOfWork _unitOfWork;
    public BasketRepository(IConnectionMultiplexer redis, IUnitOfWork unitOfWork)
    {
      _unitOfWork = unitOfWork;
      _database = redis.GetDatabase();
    }

    public async Task<bool> DeleteBasketAsync(string basketId)
    {
      return await _database.KeyDeleteAsync(basketId);
    }

    public async Task<CustomerBasket> GetBasketAsync(string basketId)
    {
      var data = await _database.StringGetAsync(basketId);

      return data.IsNullOrEmpty ? null : JsonSerializer.Deserialize<CustomerBasket>(data);
    }

    public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
    {
      if (basket.Items != null)
      {
        foreach (var basketItem in basket.Items)
        {
          if (basketItem.ChildProducts.Count > 0)
          {
            for (int i = 0; i < basketItem.ChildProducts.Count; i++)
            {
              var childProductId = basketItem.ChildProducts[i].FirstOrDefault().Value;
              var childProduct = await _unitOfWork.Repository<ChildProduct>().GetByIdAsync(childProductId);
              basketItem.ProductDescription += childProduct.Name + Environment.NewLine;
            }
          }
        }
      }

      var created = await _database.StringSetAsync(basket.Id, JsonSerializer.Serialize(basket), TimeSpan.FromDays(1));
      if (!created) return null;
      return await GetBasketAsync(basket.Id);
    }
  }
}