using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace API.Extensions
{
  public static class HttpExtensions
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="response"></param>
    /// <param name="currentPage"></param>
    /// <param name="itemsPerPage"></param>
    /// <param name="totalItems"></param>
    /// <param name="totalPages"></param>
    public static void AddPaginationHeader(this HttpResponse response, int currentPage, int itemsPerPage,
      int totalItems, int totalPages)
    {
      var paginationHeader = new
      {
        currentPage,
        itemsPerPage,
        totalItems,
        totalPages
      };

      // add a custom header
      response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader));
    }
  }
}