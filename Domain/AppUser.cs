using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Domain
{
  /// <summary>
  /// IdentityUser class comes with a lot of properties like UserName, Email, Password, etc
  /// so we don't have to create them manually
  /// So we only add the additional properties 
  /// </summary>
  public class AppUser : IdentityUser
  {
    public string DisplayName { get; set; }
    public string Bio { get; set; }
    public ICollection<ActivityAttendee> Activities { get; set; }

    public ICollection<Photo> Photos { get; set; }
    
    public ICollection<UserFollowing> Followings { get; set; }
    public ICollection<UserFollowing> Followers { get; set; }
  }
}