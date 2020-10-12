using System.ComponentModel.DataAnnotations;

namespace API.Dtos
{
  public class RegisterDto
  {
    [Required]
    public string DisplayName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [RegularExpression("^(?=(.*[a-zA-Z].*){2,})(?=.*\\d.*)(?=.*\\W.*)[a-zA-Z0-9\\S]{8,15}$",
       ErrorMessage = "Password (8 - 15 characters) must have at least two letters, one number, one special character, space is not allowed.")]
    public string Password { get; set; }
  }
}