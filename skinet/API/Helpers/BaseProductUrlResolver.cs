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
      var photo = source.Photos.FirstOrDefault(x => x.IsMain);

      if (photo != null)
      {
        return _config["ApiUrl"] + photo.PictureUrl;
      }

      return _config["ApiUrl"] + "images/products/azure-logo.png";
    }
  }
}