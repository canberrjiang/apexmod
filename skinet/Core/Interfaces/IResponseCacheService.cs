using System;
using System.Threading.Tasks;

namespace Core.Interfaces
{
  public interface IResponseCacheService
  {
    Task CacheResponseAsync(string cacheKey, object response, TimeSpan timetoleave);
    Task<string> GetCachedResponseAsync(string cacheKey);
  }
}