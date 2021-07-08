using System;

namespace Application.Comments
{
  public class CommentDto
  {
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Body { get; set; }
    
    // properties for the author of the comment
    public string Username { get; set; }
    public string DisplayName { get; set; }
    public string Image { get; set; }
  }
}