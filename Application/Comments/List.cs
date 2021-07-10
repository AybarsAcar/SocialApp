using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Comments
{
  /// <summary>
  /// query that returns the comments on an activity
  /// based on the activity id
  /// </summary>
  public class List
  {
    public class Query : IRequest<Result<List<CommentDto>>>
    {
      public Guid ActivityId { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result<List<CommentDto>>>
    {
      private readonly DataContext _context;
      private readonly IMapper _mapper;

      public Handler(DataContext context, IMapper mapper)
      {
        _context = context;
        _mapper = mapper;
      }

      public async Task<Result<List<CommentDto>>> Handle(Query request, CancellationToken cancellationToken)
      {
        // get a list of comments for a particular activity
        // order them by a particular date descending to get the newest comment at the top
        // project them to comment dto
        // execute tolist
        var comments = await _context.Comments
          .Where(x => x.Activity.Id == request.ActivityId)
          .OrderByDescending(x => x.CreatedAt)
          .ProjectTo<CommentDto>(_mapper.ConfigurationProvider)
          .ToListAsync();

        return Result<List<CommentDto>>.Success(comments);
      }
    }
  }
}