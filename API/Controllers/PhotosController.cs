using System.Threading.Tasks;
using Application.Photos;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
  /// <summary>
  /// /api/photos
  /// </summary>
  public class PhotosController : BaseApiController
  {
    /// <summary>
    /// Add photo end point
    /// </summary>
    /// <param name="command">command specifically needs to be [FromForm]</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Add([FromForm] Add.Command command)
    {
      return HandleResult(await Mediator.Send(command));
    }

    
    /// <summary>
    /// Delete photo end point
    /// </summary>
    /// <param name="id">cloudinary id</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
      return HandleResult(await Mediator.Send(new Delete.Command {Id = id}));
    }
  }
}