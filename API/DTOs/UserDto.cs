namespace API.DTOs
{
  /**
   * what user info do we want to send back to the user
   * after they are logged in
   */
  public class UserDto
  {
    public string DisplayName { get; set; }
    public string Token { get; set; }
    public string Username { get; set; }
    public string Image { get; set; }
  }
}