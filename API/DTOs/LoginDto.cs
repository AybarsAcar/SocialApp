namespace API.DTOs
{
  /**
   * Login info required from the user
   */
  public class LoginDto
  {
    public string Email { get; set; }
    public string Password { get; set; }
  }
}