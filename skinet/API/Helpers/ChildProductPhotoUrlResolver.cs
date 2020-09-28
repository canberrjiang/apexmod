using System.Linq;
using API.Dtos;
using AutoMapper;
using Core.Entities;
using Microsoft.Extensions.Configuration;

namespace API.Helpers
{
  public class ChildProductPhotoUrlResolver : IValueResolver<ChildProduct, ChildProductToReturnDto, string>
  {
    private readonly IConfiguration _config;
    public ChildProductPhotoUrlResolver(IConfiguration config)
    {
      _config = config;
    }

    public string Resolve(ChildProduct source, ChildProductToReturnDto destination, string destMember, ResolutionContext context)
    {
      if (!string.IsNullOrEmpty(source.PictureUrl))
      {
        return _config["ApiUrl"] + source.PictureUrl;
      }

      return _config["ApiUrl"] + "images/products/placeholder.png";
    }
  }
}