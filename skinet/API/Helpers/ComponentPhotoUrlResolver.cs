using API.Dtos;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Core.Entities;

namespace API.Helpers
{
  public class ComponentPhotoUrlResolver : IValueResolver<ComponentPhoto, ComponentPhotoToReturnDto, string>
  {
    private readonly IConfiguration _config;

    public ComponentPhotoUrlResolver(IConfiguration config)
    {
      _config = config;
    }

    public string Resolve(ComponentPhoto source, ComponentPhotoToReturnDto destination, string destMember, ResolutionContext context)
    {
      if (!string.IsNullOrEmpty(source.PictureUrl))
      {
        return _config["ApiUrl"] + source.PictureUrl;
      }

      return null;
    }
  }
}