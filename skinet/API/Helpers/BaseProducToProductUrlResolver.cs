using System.Linq;
using API.Dtos;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Core.Entities;

namespace API.Helpers
{
  public class BaseProducToProductUrlResolver : IValueResolver<BaseProduct, ProductToReturnDto, string>
  {
    private readonly IConfiguration _config;
    public BaseProducToProductUrlResolver(IConfiguration config)
    {
      _config = config;
    }

    public string Resolve(BaseProduct source, ProductToReturnDto destination, string destMember, ResolutionContext context)
    {
      if (!string.IsNullOrEmpty(source.PictureUrl))
      {
        return _config["ApiUrl"] + source.PictureUrl;
      }

      return _config["ApiUrl"] + "images/products/placeholder.png";
    }
  }
}