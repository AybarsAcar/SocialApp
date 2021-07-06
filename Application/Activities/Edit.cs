using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Activities
{ 
  /// <summary>
  /// Edit Activity handler
  /// </summary>
  public class Edit
  {
    public class Command : IRequest<Result<Unit>>
    {
      // send activity object from the client side
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
      private readonly IMapper _mapper;

      public Handler(DataContext context, IMapper mapper)
      {
        _context = context;
        _mapper = mapper;
      }
      
      public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
      {
        // get the activity from db
        var activity = await _context.Activities.FindAsync(request.Activity.Id);

        if (activity == null) return null;

        // if no title is sent down assign it to whatever it is in the db
        // activity.Title = request.Activity.Title ?? activity.Title;

        // map from: received from the client -> to: the one in db
        _mapper.Map(request.Activity, activity);
        
        var result = await _context.SaveChangesAsync() > 0;

        if (!result)
        {
          return Result<Unit>.Failure("Failed to update activity");
        }
        
        return Result<Unit>.Success(Unit.Value);
      }
    }
  }
}