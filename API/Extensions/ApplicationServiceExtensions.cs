using Application.Activities;
using Application.Core;
using Application.Interfaces;
using Infrastructure.Photos;
using Infrastructure.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Persistence;

namespace API.Extensions
{
  public static class ApplicationServiceExtensions
  {
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
      services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "API", Version = "v1"}); });

      // services.AddDbContext<DataContext>(opt => { opt.UseSqlite(config.GetConnectionString("DefaultConnection")); });
      services.AddDbContext<DataContext>(opt => { opt.UseNpgsql(config.GetConnectionString("DefaultConnection")); });

      services.AddCors(opt =>
      {
        opt.AddPolicy("CorsPolicy",
          policy =>
          {
            policy
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .WithOrigins("http://localhost:3000");
          });
      });

      // tell mediator where to find the handlers
      services.AddMediatR(typeof(List.Handler).Assembly);

      // tell automapper where to find the mapper profiles
      services.AddAutoMapper(typeof(MappingProfiles).Assembly);

      services.AddScoped<IUserAccessor, UserAccessor>();

      services.AddScoped<IPhotoAccessor, PhotoAccessor>();

      // add cloudinary to service container
      // config grabs from application.json file
      services.Configure<CloudinarySettings>(config.GetSection("Cloudinary"));

      services.AddSignalR();

      return services;
    }
  }
}