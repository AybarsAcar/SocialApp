namespace Application.Photos
{
  /// <summary>
  /// properties we get from the Cloudinary upload Result
  /// </summary>
  public class PhotoUploadResult
  {
    public string PublicId { get; set; }
    public string Url { get; set; }
  }
}