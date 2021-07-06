namespace Infrastructure.Photos
{
  /// <summary>
  /// Configuration properties for our Cloudinary account keys
  /// </summary>
  public class CloudinarySettings
  {
    public string CloudName { get; set; }
    public string ApiKey { get; set; }
    public string ApiSecret { get; set; }
  }
}