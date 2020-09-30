using System.Linq;
using API.Dtos;
using AutoMapper;
using Core.Entities;
using Microsoft.Extensions.Configuration;

namespace API.Helpers
{
  public class ChildProductsToReturnUrlResolver : IValueResolver<ProductProduct, ChildProductsToReturnDto, string>
  {
    private readonly IConfiguration _config;
    public ChildProductsToReturnUrlResolver(IConfiguration config)
    {
      _config = config;
    }

    public string Resolve(ProductProduct source, ChildProductsToReturnDto destination, string destMember, ResolutionContext context)
    {
      if (source.ChildProduct != null)
      {
        var photo = source.ChildProduct.Photos.FirstOrDefault(x => x.IsMain);

        if (photo != null)
        {
          return _config["ApiUrl"] + photo.PictureUrl;
        }
      }


      return _config["ApiUrl"] + "images/products/placeholder.png";
    }
  }
}