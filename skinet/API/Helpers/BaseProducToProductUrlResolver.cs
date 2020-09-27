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
      var photo = source.Photos.FirstOrDefault(x => x.IsMain);

      if (photo != null)
      {
        return _config["ApiUrl"] + photo.PictureUrl;
      }

      return _config["ApiUrl"] + "images/products/placeholder.png";
    }
  }
}