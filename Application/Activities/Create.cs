using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
  /**
   * Command to create an activity
   */
  public class Create
  {
    /**
     * no type parameter on the IRequest
     * because we are not returning anything from a Command - CQRS Pattern
     */
    public class Command : IRequest<Result<Unit>>
    {
      // what we receive as a parameter from our API
      public Activity Activity { get; set; }
    }

    /**
     * our interceptor class for Error handling
     * Data checking
     */
    public class CommandValidator : AbstractValidator<Command>
    {
      public CommandValidator()
      {
        RuleFor(x => x.Activity).SetValidator(new ActivityValidator());
      }
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
        // get user
        var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());

        // create new attendee from that user
        var attendee = new ActivityAttendee
        {
          AppUser = user,
          Activity = request.Activity,
          IsHost = true
        };

        request.Activity.Attendees.Add(attendee);

        _context.Activities.Add(request.Activity);

        var result = await _context.SaveChangesAsync() > 0;

        if (!result) return Result<Unit>.Failure("Failed to create activity");

        return Result<Unit>.Success(Unit.Value);
      }
    }
  }
}