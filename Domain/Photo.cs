namespace Domain
{
  /// <summary>
  /// Photo information we store in our database
  /// </summary>
  public class Photo
  {
    // will be the publicId we get from cloudinary
    public string Id { get; set; }

    // ulr we get from cloudinary
    public string Url { get; set; }
    
    public bool IsMain { get; set; }
  }
}