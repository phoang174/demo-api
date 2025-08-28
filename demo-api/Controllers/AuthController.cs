using Application.Dtos;
using Application.IService;
using Application.Users.Commands;
using demo_api.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
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
            if (result.IsFailure)
            {
                return Unauthorized(new { message = "Wrong username or password" });
            }

            SetRefreshTokenCookie(result.Value.RefreshToken);

            return Ok(new
            {
                user = result.Value.User,
                accessToken = result.Value.AccessToken
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

            var res = result.Value;
            SetRefreshTokenCookie(res!.RefreshToken);

            return Ok(new
            {
                user = res.User,
                accessToken = res.AccessToken
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
                userId=77, 
                accessToken= accessToken
            });
            if (result.IsFailure)
                throw new CustomException(result.Error.Message, result.Error.Code, result.Error.Detail);

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
