namespace Application.Activities
{
  /// <summary>
  /// To return with the ActivitiesDto
  /// so we don't need to send the whole Profile information for efficiency
  /// </summary>
  public class AttendeeDto
  {
    public string Username { get; set; }
    public string DisplayName { get; set; }
    public string Bio { get; set; }
    public string Image { get; set; }
    
    public bool Following { get; set; }
    public int FollowerCount { get; set; }
    public int FollowingCount { get; set; }
  }
}