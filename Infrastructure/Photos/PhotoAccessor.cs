using System;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Photos;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Infrastructure.Photos
{
  /// <summary>
  /// accesses to the Cloudinary Cloud Services
  /// </summary>
  public class PhotoAccessor : IPhotoAccessor
  {
    private readonly Cloudinary _cloudinary;

    public PhotoAccessor(IOptions<CloudinarySettings> config)
    {
      // get the cloudinary user account
      var account = new Account(config.Value.CloudName, config.Value.ApiKey, config.Value.ApiSecret);
      
      // get the connection
      _cloudinary = new Cloudinary(account);
    }

    /// <summary>
    /// sends a request to cloudinary to upload
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public async Task<PhotoUploadResult> AddPhoto(IFormFile file)
    {
      if (file.Length > 0)
      {
        await using var stream = file.OpenReadStream();

        // specify the upload parameters
        var uploadParams = new ImageUploadParams
        {
          File = new FileDescription(file.FileName, stream),
          Transformation = new Transformation().Height(500).Width(500).Crop("fill"),
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error != null)
        {
          throw new Exception(uploadResult.Error.Message);
        }

        return new PhotoUploadResult
        {
          PublicId = uploadResult.PublicId,
          Url =  uploadResult.SecureUrl.ToString()
        };
      }

      return null;
    }

    /// <summary>
    /// Request to cloudinary to delete the image
    /// </summary>
    /// <param name="publicId">cloudinary public id of the image</param>
    /// <returns></returns>
    public async Task<string> DeletePhoto(string publicId)
    {
      var deleteParams = new DeletionParams(publicId);
      var result = await _cloudinary.DestroyAsync(deleteParams);

      return result.Result == "ok" ? result.Result : null;
    }
  }
}