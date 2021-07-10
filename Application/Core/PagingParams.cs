using Application.Profiles;

namespace Application.Core
{
  /// <summary>
  /// 
  /// </summary>
  public class PagingParams
  {
    private const int MaxPageSize = 50;

    public int PageNumber { get; set; } = 1;


    private int _pageSize = 10; // default page size if no params is passed in
    public int PageSize
    {
      get => _pageSize;
      set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }
  }
}