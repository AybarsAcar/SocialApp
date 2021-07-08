using System;

namespace Domain
{
  /// <summary>
  /// our comment entity
  /// Stored in UtcNow defaulted to current time the comment is made
  /// </summary>
  public class Comment
  {
    public int Id { get; set; }
    public string Body { get; set; }
    public AppUser Author { get; set; }
    public Activity Activity { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  }
}