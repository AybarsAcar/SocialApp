using System.Threading.Tasks;
using Application.Followers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
  /// <summary>
  /// 
  /// </summary>
  public class FollowController : BaseApiController
  {
    /// <summary>
    /// end point to follow / unfollow a user
    /// logged in user received from the Claims
    /// </summary>
    /// <param name="username">Target user being followed</param>
    /// <returns></returns>
    [HttpPost("{username}")]
    public async Task<IActionResult> Follow(string username)
    {
      return HandleResult(await Mediator.Send(new FollowToggle.Command {TargetUsername = username}));
    }

    /// <summary>
    /// End point to return the following and follower profiles
    /// </summary>
    /// <param name="username">user we want to get the information from</param>
    /// <param name="predicate">following or followers</param>
    /// <returns></returns>
    [HttpGet("{username}")]
    public async Task<IActionResult> GetFollowings(string username, [FromQuery] string predicate)
    {
      return HandleResult(await Mediator.Send(new List.Query {Username = username, Predicate = predicate}));
    }
}
}