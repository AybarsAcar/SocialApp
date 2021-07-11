using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Persistence;

namespace Application.Activities
{
  /// <summary>
  /// Mediator Pattern class
  /// It is used to list the activities
  /// </summary>
  public class List
  {
    public class Query : IRequest<Result<PagedList<ActivityDto>>>
    {
      public ActivityParams Params { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result<PagedList<ActivityDto>>>
    {
      private readonly DataContext _context;
      private readonly IMapper _mapper;
      private readonly IUserAccessor _userAccessor;

      public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
      {
        _context = context;
        _mapper = mapper;
        _userAccessor = userAccessor;
      }

      public async Task<Result<PagedList<ActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
      {
        // defer the query to the database
        var query = _context.Activities
          .Where(d => d.Date >= request.Params.StartDate) // send future activities by default
          .OrderBy(d => d.Date)
          .ProjectTo<ActivityDto>(_mapper.ConfigurationProvider, new {currentUsername = _userAccessor.GetUsername()})
          .AsQueryable();

        // show the events that the currently logged in user going
        if (request.Params.IsGoing && !request.Params.IsHost)
        {
          query = query.Where(x => x.Attendees.Any(a => a.Username == _userAccessor.GetUsername()));
        }

        // return the hosting activities
        if (request.Params.IsHost && !request.Params.IsGoing)
        {
          query = query.Where(x => x.HostUsername == _userAccessor.GetUsername());
        }

        return Result<PagedList<ActivityDto>>.Success(
          await PagedList<ActivityDto>.CreateAsync(query, request.Params.PageNumber, request.Params.PageSize)
        );
      }
    }
  }
}