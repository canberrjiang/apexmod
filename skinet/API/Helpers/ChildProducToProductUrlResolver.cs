using System.Linq;
using API.Dtos;
using AutoMapper;
using Core.Entities;
using Microsoft.Extensions.Configuration;

namespace API.Helpers
{
  public class ChildProducToProductUrlResolver : IValueResolver<ChildProduct, ProductToReturnDto, string>
  {
    private readonly IConfiguration _config;
    public ChildProducToProductUrlResolver(IConfiguration config)
    {
      _config = config;
    }

    public string Resolve(ChildProduct source, ProductToReturnDto destination, string destMember, ResolutionContext context)
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