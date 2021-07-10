using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Followers
{
  /// <summary>
  /// to follow and unfollow an AppUser
  /// </summary>
  public class FollowToggle
  {
    public class Command : IRequest<Result<Unit>>
    {
      public string TargetUsername { get; set; }
    }

    public class Handler : IRequestHandler<Command, Result<Unit>>
    {
      private readonly DataContext _context;
      private readonly IUserAccessor _userAccessor;

      public Handler(DataContext context, IUserAccessor userAccessor)
      {
        _context = context;
        _userAccessor = userAccessor;
      }

      public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
      {
        // get the users
        // the currently logged in user
        var observer = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());

        // target user where currently logged in user is either following or unfollowing
        var target = await _context.Users.FirstOrDefaultAsync(x => x.UserName == request.TargetUsername);

        if (target == null) return null;

        var following = await _context.UserFollowings.FindAsync(observer.Id, target.Id);

        if (following == null)
        {
          // current action is user following
          following = new UserFollowing {Observer = observer, Target = target};

          _context.UserFollowings.Add(following);
        }
        else
        {
          // current action is user unfollowing
          _context.UserFollowings.Remove(following);
        }

        var success = await _context.SaveChangesAsync() > 0;

        if (success)
        {
          return Result<Unit>.Success(Unit.Value);
        }

        return Result<Unit>.Failure("Error updating following");
      }
    }
  }
}