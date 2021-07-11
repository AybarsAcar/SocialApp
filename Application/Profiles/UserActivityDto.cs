using System;
using System.Text.Json.Serialization;

namespace Application.Profiles
{
  /// <summary>
  /// DTO to send to the user profile
  /// information displayed on the cards on the user profile events page
  /// </summary>
  public class UserActivityDto
  {
    public Guid ActivityId { get; set; }
    public string Title { get; set; }
    public string Category { get; set; }
    public DateTime Date { get; set; }
    
    [JsonIgnore] // so we don't send this to the client
    public string HostUsername { get; set; }
  }
}