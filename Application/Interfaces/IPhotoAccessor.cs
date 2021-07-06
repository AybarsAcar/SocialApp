using System.Threading.Tasks;
using Application.Photos;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
  public interface IPhotoAccessor
  {
    Task<PhotoUploadResult> AddPhoto(IFormFile file);

    // public id is the cloudinary public id
    Task<string> DeletePhoto(string publicId);
  }
}