using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API.Middleware
{
  /**
   * our custom middleware to handle the exceptions
   * we pass next
   */
  public class ExceptionMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
      _next = next;
      _logger = logger;
      _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
      try
      {
        // if no exception, we move to the next in the middleware
        await _next(context);
      }
      catch (Exception e)
      {
        // we have an exception to be handler
        _logger.LogError(e, e.Message);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;

        var response = _env.IsDevelopment()
          ? new AppException(context.Response.StatusCode, e.Message, e.StackTrace?.ToString())
          : new AppException(context.Response.StatusCode, "Server Error"); // generic response for production


        // set up our json serializer so we send in camel case rather than title caase
        var options = new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase};

        var json = JsonSerializer.Serialize(response, options);

        await context.Response.WriteAsync(json);
      }
    }
  }
}