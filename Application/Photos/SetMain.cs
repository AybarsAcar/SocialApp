using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
  /// <summary>
  /// Command to set the main photo
  /// </summary>
  public class SetMain
  {
    public class Command : IRequest<Result<Unit>>
    {
      public string Id { get; set; } // id of the photo we want to set as main
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
        // get the user
        var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(
          x => x.UserName == _userAccessor.GetUsername());

        if (user == null) return null;

        // get the photo
        var photo = user.Photos.FirstOrDefault(x => x.Id == request.Id);

        if (photo == null) return null;

        var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

        
        // switch the main flag
        if (currentMain != null)
        {
          currentMain.IsMain = false;
        }

        photo.IsMain = true;
        
        // update the db
        var success = await _context.SaveChangesAsync() > 0;

        if (success)
        {
          return Result<Unit>.Success(Unit.Value);
        }

        return Result<Unit>.Failure("Problem setting main photo");
      }
    }
  }
}