using System.Linq;
using API.Dtos;
using AutoMapper;
using Core.Entities;
using Microsoft.Extensions.Configuration;

namespace API.Helpers
{
  public class BaseProductUrlResolver : IValueResolver<BaseProduct, BaseProductToReturnDto, string>
  {
    private readonly IConfiguration _config;
    public BaseProductUrlResolver(IConfiguration config)
    {
      _config = config;
    }

    public string Resolve(BaseProduct source, BaseProductToReturnDto destination, string destMember, ResolutionContext context)
    {
      if (!string.IsNullOrEmpty(source.PictureUrl))
      {
        return _config["ApiUrl"] + source.PictureUrl;
      }

      return _config["ApiUrl"] + "images/products/placeholder.png";
    }
  }
}