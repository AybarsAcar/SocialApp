using API.Extensions;
using Application.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace API.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class BaseApiController : ControllerBase
  {
    private IMediator _mediator;

    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="result"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    protected ActionResult HandleResult<T>(Result<T> result)
    {
      if (result == null)
      {
        return NotFound();
      }

      if (result.IsSuccess && result.Value != null)
      {
        return Ok(result.Value);
      }

      if (result.IsSuccess && result.Value == null)
      {
        return NotFound();
      }

      return BadRequest(result.Error);
    }

    /// <summary>
    /// to handle paginated responses
    /// </summary>
    /// <param name="result"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    protected ActionResult HandlePagedResult<T>(Result<PagedList<T>> result)
    {
      if (result == null)
      {
        return NotFound();
      }

      if (result.IsSuccess && result.Value != null)
      {
        // add the custom header to the response
        Response.AddPaginationHeader(result.Value.CurrentPage, result.Value.PageSize, result.Value.TotalCount,
          result.Value.TotalPages);

        return Ok(result.Value);
      }

      if (result.IsSuccess && result.Value == null)
      {
        return NotFound();
      }

      return BadRequest(result.Error);
    }
  }
}