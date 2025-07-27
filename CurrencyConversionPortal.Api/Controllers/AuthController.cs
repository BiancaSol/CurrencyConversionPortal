namespace CurrencyConversionPortal.Api.Controllers
{
    using CurrencyConversionPortal.Api.DTOs;
    using CurrencyConversionPortal.Core.Services;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterUserDto registerUserDto)
        {
            var success = _userService.Register(registerUserDto.Username, registerUserDto.Password);

            if (!success)
                return Conflict("Username already exists.");

            return Created(string.Empty, new
            {
                username = registerUserDto.Username
            });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto)
        {
            if (!_userService.ValidateCredentials(loginUserDto.Username, loginUserDto.Password))
                return Unauthorized("Invalid username or password.");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, loginUserDto.Username)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal
            );

            return Ok(new { message = "Login successful." });
        }

        [Authorize(Policy = "StandardUser")]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { message = "Logged out successfully." });
        }
    }
}