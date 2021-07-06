using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
  /**
   * info required to register a user
   */
  public class RegisterDto
  {
    [Required]
    public string DisplayName { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    public string Username { get; set; }
    
    [Required]
    [RegularExpression("(?=.*\\d)(?=.*[a-z])(?=/*[A-Z]).{4,8}$", ErrorMessage = "Password is weak")]
    public string Password { get; set; }
  }
}