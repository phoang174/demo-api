using Application.Dtos;
using Application.IService;
using Application.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace demo_api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ISender _sender;

        public AuthController(ISender sender)
        {
            _sender = sender;

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginCommand body)
        {
            var result = await _sender.Send(body);
            if (result == null)
            {
                return Unauthorized(new { message = "Wrong username or password" });
            }

            SetRefreshTokenCookie(result.RefreshToken);

            return Ok(new
            {
                user = result.User,
                accessToken = result.AccessToken
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken()
        {
            if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken) || string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(new { message = "Refresh token not found in cookies" });
            }

            var result = await _sender.Send(new RefreshTokenCommand
            {
                refreshToken = refreshToken
            });
            if (result == null)
            {
                return Unauthorized(new { message = "Invalid refresh token" });
            }

            SetRefreshTokenCookie(result.RefreshToken);

            return Ok(new
            {
                user = result.User,
                accessToken = result.AccessToken
            });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return BadRequest(new { message = "Invalid user id" });

            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return BadRequest(new { message = "Missing or invalid Authorization header" });

            var accessToken = authHeader.Substring("Bearer ".Length).Trim();

            var result = await _sender.Send(new LogoutCommand
            {
                userId=userId, 
                accessToken= accessToken
            });
            if (!result)
                return BadRequest(new { message = "Logout failed" });

            Response.Cookies.Delete("refreshToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = IsProduction(),
                SameSite = SameSiteMode.None
            });

            return Ok(new { message = "Logout successful" });
        }

        private void SetRefreshTokenCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, 
                SameSite = SameSiteMode.None, 
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

        private bool IsProduction()
        {
            return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production";
        }
    }
}
