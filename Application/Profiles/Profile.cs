namespace Application.Profiles
{
  /**
   * Used for the user profile and our users that we return with the activities
   */
  public class Profile
  {
    public string Username { get; set; }
    public string DisplayName { get; set; }
    public string Bio { get; set; }
    public string Image { get; set; }
  }
}