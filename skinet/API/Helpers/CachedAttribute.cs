using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace API.Helpers
{
  public class CachedAttribute : Attribute, IAsyncActionFilter
  {
    private readonly int _timeToLeaveSeconds;
    public CachedAttribute(int timeToLeaveSeconds)
    {
      _timeToLeaveSeconds = timeToLeaveSeconds;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
      // Get request query
      var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();

      // build cache key based on the request query
      var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);
      // get the cached object 
      var cachedResponse = await cacheService.GetCachedResponseAsync(cacheKey);

      if (!string.IsNullOrEmpty(cachedResponse))
      {
        var contentResult = new ContentResult
        {
          Content = cachedResponse,
          ContentType = "application/json",
          StatusCode = 200
        };
        context.Result = contentResult;

        return;
      }

      var executedContext = await next(); // move to controller


      if (executedContext.Result is OkObjectResult okObjectResult)
      {
        await cacheService.CacheResponseAsync(cacheKey, okObjectResult.Value, TimeSpan.FromSeconds(_timeToLeaveSeconds));
      }
    }

    private string GenerateCacheKeyFromRequest(HttpRequest request)
    {
      var keyBuilder = new StringBuilder();
      keyBuilder.Append($"{request.Path}");

      foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
      {
        keyBuilder.Append($"|{key}-{value}");
      }

      return keyBuilder.ToString();
    }
  }
}