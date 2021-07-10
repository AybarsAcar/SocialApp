using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Application.Core
{
  /// <summary>
  /// a paginated list
  /// List will be extended to give pagination properties
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class PagedList<T> : List<T>
  {
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    
    public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
    {
      CurrentPage = pageNumber;
      TotalPages = (int) Math.Ceiling(count / (double) pageSize);
      PageSize = pageSize;
      TotalCount = count;
      AddRange(items);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source">query that will go to database</param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <returns>Paginated List of Result</returns>
    public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
    {
      // get the count of the items in the db before pagination takes place
      var count = await source.CountAsync();

      var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

      return new PagedList<T>(items, count, pageNumber, pageSize);
    }
  }
}