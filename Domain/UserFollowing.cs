namespace Domain
{
  /// <summary>
  /// the join entity
  /// </summary>
  public class UserFollowing
  {
    // the follower
    public string ObserverId { get; set; }
    public AppUser Observer { get; set; }

    // the followee
    public string TargetId { get; set; }
    public AppUser Target { get; set; }
  }
}