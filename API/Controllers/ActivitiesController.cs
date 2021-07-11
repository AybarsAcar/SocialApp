using System;
using System.Threading.Tasks;
using Application.Activities;
using Application.Core;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
  public class ActivitiesController : BaseApiController
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="activityParams">activity params that inherits from PagingParams</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetActivities([FromQuery] ActivityParams activityParams)
    {
      var result = await Mediator.Send(new List.Query {Params = activityParams});
      return HandlePagedResult(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetActivity(Guid id)
    {
      var result = await Mediator.Send(new Details.Query {Id = id});
      return HandleResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateActivity([FromBody] Activity activity)
    {
      return HandleResult(await Mediator.Send(new Create.Command {Activity = activity}));
    }

    [Authorize(Policy = "IsActivityHost")] // add our custom Auth policy
    [HttpPut("{id}")]
    public async Task<IActionResult> EditActivity(Guid id, Activity activity)
    {
      activity.Id = id;

      return HandleResult(await Mediator.Send(new Edit.Command {Activity = activity}));
    }

    [Authorize(Policy = "IsActivityHost")] // add our custom Auth policy
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteActivity(Guid id)
    {
      return HandleResult(await Mediator.Send(new Delete.Command {Id = id}));
    }

    [HttpPost("{id}/attend")]
    public async Task<IActionResult> Attend(Guid id)
    {
      return HandleResult(await Mediator.Send(new UpdateAttendance.Command {Id = id}));
    }
  }
}