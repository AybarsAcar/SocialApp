using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
  public class TokenService
  {
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
      _config = config;
    }

    public string CreateToken(AppUser user)


    {
      // add as many claims as you mind
      // whatever you want to track
      // this token will be sent for authorisation at each request so make sure it is not bulky
      // we can create additional tokens for other information or cookies
      var claims = new List<Claim>
      {
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Email, user.Email)
      };

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["TokenKey"]));

      var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.UtcNow.AddMinutes(10),
        SigningCredentials = credentials,
      };

      var tokenHandler = new JwtSecurityTokenHandler();

      var token = tokenHandler.CreateToken(tokenDescriptor);

      return tokenHandler.WriteToken(token); // this method returns an actual JWT token
    }

    public RefreshToken GenerateRefreshToken()
    {
      var randomNumber = new byte[32];

      using var rng = RandomNumberGenerator.Create();
      rng.GetBytes(randomNumber);
      return new RefreshToken {Token = Convert.ToBase64String(randomNumber)};
    }
  }
}