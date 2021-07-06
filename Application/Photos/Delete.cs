using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Activities;
using Application.Core;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
  public class Delete
  {
    public class Command : IRequest<Result<Unit>>
    {
      public string Id { get; set; } // has to match the cloudinary public id 
    }

    public class Handler : IRequestHandler<Command, Result<Unit>>
    {
      private readonly DataContext _context;
      private readonly IPhotoAccessor _photoAccessor;
      private readonly IUserAccessor _userAccessor;

      public Handler(DataContext context, IPhotoAccessor photoAccessor, IUserAccessor userAccessor)
      {
        _context = context;
        _photoAccessor = photoAccessor;
        _userAccessor = userAccessor;
      }

      public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
      {
        // get the user including the photos collection
        var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(
          x => x.UserName == _userAccessor.GetUsername()
        );

        if (user == null) return null;

        // get the photo from the memory
        var photo = user.Photos.FirstOrDefault(x => x.Id == request.Id);

        if (photo == null) return null;

        if (photo.IsMain)
        {
          return Result<Unit>.Failure("You cannot delete your main photo");
        }

        // attempt to delete from cloudinary
        var result = await _photoAccessor.DeletePhoto(request.Id);

        if (result == null) return Result<Unit>.Failure("Problem deleting the photo from Cloudinary");

        user.Photos.Remove(photo);

        var success = await _context.SaveChangesAsync() > 0;

        if (success)
        {
          return Result<Unit>.Success(Unit.Value);
        }

        return Result<Unit>.Failure("Problem deleting the photo from API");
      }
    }
  }
}