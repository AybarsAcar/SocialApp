using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Extensions;
using API.Middleware;
using API.SignalR;
using Application.Activities;
using Application.Core;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Persistence;

namespace API
{
  public class Startup
  {
    private readonly IConfiguration _config;

    public Startup(IConfiguration config)
    {
      _config = config;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    // Dependency injection container
    public void ConfigureServices(IServiceCollection services)
    {
      // add the fluent validators to our controllers
      services.AddControllers(opt =>
        {
          // every single end point now requires authentication unless specified otherwise
          var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
          opt.Filters.Add(new AuthorizeFilter(policy));
        })
        .AddFluentValidation(config => { config.RegisterValidatorsFromAssemblyContaining<Create>(); });

      // our extensions specific to this application
      services.AddApplicationServices(_config);
      services.AddIdentityServices(_config);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      // instead of the developer exception page we will be using our own exception middleware
      app.UseMiddleware<ExceptionMiddleware>();

      // add security headers
      app.UseXContentTypeOptions();
      app.UseReferrerPolicy(opt => opt.NoReferrer());
      app.UseXXssProtection(opt => opt.EnabledWithBlockMode());
      app.UseXfo(opt => opt.Deny());
      app.UseCspReportOnly(opt => opt
        .BlockAllMixedContent()
        .StyleSources(s =>
          s.Self().CustomSources("https://fonts.googleapis.com", "sha256-oFySg82XYSNiSd+Q3yfYPD/rxY6RMDMJ0KxzGG74iGM="))
        .FontSources(s => s.Self().CustomSources("https://fonts.gstatic.com", "data:"))
        .FormActions(s => s.Self())
        .FrameAncestors(s => s.Self())
        .ImageSources(s => s.Self().CustomSources("https://res.cloudinary.com", "https://www.facebook.com",
          "https://scontent-syd2-1.xx.fbcdn.net", "data:"))
        .ScriptSources(s => s.Self()
          .CustomSources("sha256-sP1Nh4NLLjzMqHUFFyxazroQUx3RmnLnSmYFIX98xaA=", "https://connect.facebook.net",
            "sha256-MCcr8tSexkJ62IYb6uYhtYVWPEIWTk91nNjq5gpv7R8=")));

      // check to see if in the developer mode
      if (env.IsDevelopment())
      {
        // app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
      }
      else
      {
        // add extra security for production
        app.Use(async (context, next) =>
        {
          context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000");
          await next.Invoke();
        });
      }

      // app.UseHttpsRedirection();

      app.UseRouting();

      // Serving Static files from wwwroot folder
      app.UseDefaultFiles();
      app.UseStaticFiles();
      // ends

      app.UseCors("CorsPolicy");

      app.UseAuthentication();

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers(); // rest end points
        endpoints.MapHub<ChatHub>("/chat"); // signalR end point

        endpoints.MapFallbackToController("Index", "Fallback");
      });
    }
  }
}