using System;
using System.Threading.Tasks;
using Application.Comments;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
  /// <summary>
  /// Web socket connection to server
  /// methods in this class are directly invoked by the client if they are connected
  /// </summary>
  public class ChatHub : Hub
  {
    private readonly IMediator _mediator;

    public ChatHub(IMediator mediator)
    {
      _mediator = mediator;
    }

    /// <summary>
    /// This method will be invoked from the client
    /// </summary>
    /// <param name="command">Comment Create Command</param>
    public async Task SendComment(Create.Command command)
    {
      var comment = await _mediator.Send(command);
      
      // group matches the activity
      // ReceiveComment is called in the client
      await Clients.Group(command.ActivityId.ToString())
        .SendAsync("ReceiveComment", comment.Value);  
    }

    /// <summary>
    /// called when a client is connected to the hub
    /// </summary>
    public override async Task OnConnectedAsync()
    {
      var httpContext = Context.GetHttpContext();
      var activityId = httpContext.Request.Query["activityId"]; // activityId is sent from the client

      await Groups.AddToGroupAsync(Context.ConnectionId, activityId);

      var result = await _mediator.Send(new List.Query {ActivityId = Guid.Parse(activityId)});

      await Clients.Caller.SendAsync("LoadComments", result.Value);
    }
  }
}