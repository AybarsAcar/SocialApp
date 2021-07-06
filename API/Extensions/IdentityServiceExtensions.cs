using System.Text;
using API.Services;
using Domain;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Persistence;

namespace API.Extensions
{
  public static class IdentityServiceExtensions
  {
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
    {
      // add the Identity Core into dependency container
      // pass in options, you can give any options you want that you dont want if different from the default options
      services.AddIdentityCore<AppUser>(opt => { opt.Password.RequireNonAlphanumeric = false; })
        .AddEntityFrameworkStores<DataContext>()
        .AddSignInManager<SignInManager<AppUser>>();


      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
      // add the authentication strategy
      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(opt =>
        {
          opt.TokenValidationParameters = new TokenValidationParameters
          {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = false,
            ValidateAudience = false
          };
        });
      
      // auth policy
      services.AddAuthorization(opt =>
      {
        opt.AddPolicy("IsActivityHost", policy =>
        {
          policy.Requirements.Add(new IsHostRequirement());
        });
      });

      // AddTransient - lasts as long as the method is running
      services.AddTransient<IAuthorizationHandler, IsHostRequirementHandler>();

      services.AddScoped<TokenService>();
      
      return services;
    }
  }
}