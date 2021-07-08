using System.Threading.Tasks;
using Application.Profiles;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
  public class ProfilesController : BaseApiController
  {
    /// <summary>
    /// End point to return user profile
    /// </summary>
    /// <param name="username">unique</param>
    /// <returns></returns>
    [HttpGet("{username}")]
    public async Task<IActionResult> GetProfile(string username)
    {
      return HandleResult(await Mediator.Send(new Details.Query{Username = username}));
    }

    [HttpPut]
    public async Task<IActionResult> Edit(Edit.Command command)
    {
      return HandleResult(await Mediator.Send(command));
    }
  }
}