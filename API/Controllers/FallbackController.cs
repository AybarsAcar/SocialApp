using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
  /// <summary>
  /// Fallback to serve our static files
  /// Add the fallback route to .UseEndpoints in Startup.cs
  /// localhost:5000 serves the static built React application in development
  /// </summary>
  [AllowAnonymous]
  public class FallbackController : Controller
  {
    /// <summary>
    /// serves the Javascript client application
    /// </summary>
    /// <returns>index.html</returns>
    public IActionResult Index()
    {
      return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html"), "text/HTML");
    }
  }
}