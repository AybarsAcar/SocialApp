using System;

namespace Domain
{
  /**
   * Join Table
   * for Activity - AppUser many to many relationship
   */
  public class ActivityAttendee
  {
    public string AppUserId { get; set; }
    public AppUser AppUser { get; set; }
    public Guid ActivityId { get; set; }
    public Activity Activity { get; set; }
    public bool IsHost { get; set; }
  }
}