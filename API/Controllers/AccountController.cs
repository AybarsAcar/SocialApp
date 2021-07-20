using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using API.DTOs;
using API.Services;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace API.Controllers
{
  /// <summary>
  /// this is not a thin Controller layer
  /// It also includes our authentication logic in it as well unlike other controllers
  /// </summary>
  [ApiController]
  [Route("/api/[controller]")]
  public class AccountController : ControllerBase
  {
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly TokenService _tokenService;
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;

    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
      TokenService tokenService, IConfiguration config)
    {
      _userManager = userManager;
      _signInManager = signInManager;
      _tokenService = tokenService;
      _config = config;

      _httpClient = new HttpClient
      {
        BaseAddress = new Uri("https://graph.facebook.com")
      };
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
      // get the user object from the db
      var user = await _userManager.Users.Include(p => p.Photos).FirstOrDefaultAsync(
        x => x.Email == loginDto.Email);

      if (user == null)
      {
        return Unauthorized();
      }

      var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

      if (result.Succeeded)
      {
        await SetRefreshToken(user);
        
        return CreateUserObject(user);
      }

      return Unauthorized();
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
      if (await _userManager.Users.AnyAsync(x => x.Email == registerDto.Email))
      {
        ModelState.AddModelError("email", "Email is already taken");

        return ValidationProblem(ModelState);
      }

      if (await _userManager.Users.AnyAsync(x => x.UserName == registerDto.Username))
      {
        ModelState.AddModelError("username", "Username is already taken");

        return ValidationProblem(ModelState);
      }

      // now we can register
      var user = new AppUser
      {
        DisplayName = registerDto.DisplayName,
        Email = registerDto.Email,
        UserName = registerDto.Username
      };

      var result = await _userManager.CreateAsync(user, registerDto.Password);

      if (result.Succeeded)
      {
        await SetRefreshToken(user);
        
        return CreateUserObject(user);
      }

      return BadRequest("Problem registering user");
    }

    [Authorize] // because we AllowAnonymous the whole class
    [HttpGet]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
      var user = await _userManager.Users.Include(p => p.Photos)
        .FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));

      await SetRefreshToken(user);

      return CreateUserObject(user);
    }

    /// <summary>
    /// End point to authenticate users with facebook login
    /// </summary>
    /// <param name="accessToken">access token is returned to our client from facebook</param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("fbLogin")]
    public async Task<ActionResult<UserDto>> FacebookLogin([FromQuery] string accessToken)
    {
      Console.WriteLine(accessToken);

      // verify the access token for our application
      var fbVerifyKeys = _config["Facebook:AppId"] + "|" + _config["Facebook:AppSecret"];

      var verifyToken =
        await _httpClient.GetAsync($"debug_token?input_token={accessToken}&access_token={fbVerifyKeys}");

      if (!verifyToken.IsSuccessStatusCode) return Unauthorized();

      var fbUrl = $"me?access_token={accessToken}&fields=name,email,picture.width(100).height(100)";

      var response = await _httpClient.GetAsync(fbUrl);

      if (!response.IsSuccessStatusCode) return Unauthorized();

      var content = await response.Content.ReadAsStringAsync();

      // dynamic object - only know the properties in Runtime
      // safer to create a class for Facebook login properties
      var fbInfo = JsonConvert.DeserializeObject<dynamic>(content);

      // their facebook id will be stored as their username in our db
      var username = (string) fbInfo.id;

      var user = await _userManager.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.UserName == username);

      if (user != null)
      {
        // they are already logged in before
        // they exist in our database
        return CreateUserObject(user);
      }

      // new user to our application
      // save them to our database
      user = new AppUser()
      {
        DisplayName = (string) fbInfo.name,
        Email = (string) fbInfo.email,
        UserName = (string) fbInfo.id,
        Photos = new List<Photo>
        {
          new Photo {Id = "fb_" + (string) fbInfo.id, Url = (string) fbInfo.picture.data.url, IsMain = true}
        }
      };

      // save them to our db
      var result = await _userManager.CreateAsync(user);

      if (!result.Succeeded)
      {
        return BadRequest("Problem creating the user account");
      }

      await SetRefreshToken(user);

      return CreateUserObject(user);
    }

    /// <summary>
    /// Used to refresh users token
    /// and sends back the UserDto to the client so it can be used
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpPost("refreshToken")]
    public async Task<ActionResult<UserDto>> RefreshToken()
    {
      var refreshToken = Request.Cookies["refreshToken"];

      var user = await _userManager.Users
        .Include(r => r.RefreshTokens)
        .Include(p => p.Photos)
        .FirstOrDefaultAsync(x => x.UserName == User.FindFirstValue(ClaimTypes.Name));

      if (user == null) return Unauthorized();

      var oldToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken);

      if (oldToken != null && !oldToken.IsActive) return Unauthorized();
      
      if (oldToken != null) oldToken.Revoked = DateTime.UtcNow;

      return CreateUserObject(user);
    }

    private async Task SetRefreshToken(AppUser user)
    {
      var refreshToken = _tokenService.GenerateRefreshToken();

      user.RefreshTokens.Add(refreshToken);

      await _userManager.UpdateAsync(user);

      var cookieOptions = new CookieOptions
      {
        HttpOnly = true,
        Expires = DateTimeOffset.UtcNow.AddDays(7),
      };

      Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
    }

    /// <summary>
    /// because IMapper is not used for the authentication system
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    private UserDto CreateUserObject(AppUser user)
    {
      return new UserDto
      {
        DisplayName = user.DisplayName,
        Image = user?.Photos.FirstOrDefault(x => x.IsMain)?.Url,
        Token = _tokenService.CreateToken(user),
        Username = user.UserName
      };
    }
  }
}