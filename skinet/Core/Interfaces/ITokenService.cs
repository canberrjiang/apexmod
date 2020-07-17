using Core.Entities.Identity;

namespace Core.Interfaces
{
  public interface ITokenService
  {
    string Createtoken(AppUser user);
  }
}