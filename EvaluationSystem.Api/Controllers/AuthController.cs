using EvaluationSystem.Application.DTOs.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EvaluationSystem.Application.Services.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;

namespace EvaluationSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            await _authService.RegisterAsync(dto);
            return Ok(new AuthMessageDto { message = "User registered successfully" });
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var authResponse = await _authService.LoginAsync(dto);
            Response.Cookies.Append(
                "refreshToken",
                authResponse.RefreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = authResponse.RefreshTokenExpiresAt
                });

            return Ok(new
            {
                authResponse.Token,
                authResponse.TokenExpiresAt
            });
        }
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized("Refresh token not found");

            var response = await _authService.RefreshTokenAsync(refreshToken);

            Response.Cookies.Append(
                "refreshToken",
                response.RefreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = response.RefreshTokenExpiresAt
                });

            return Ok(new
            {
                response.Token,
                response.TokenExpiresAt
            });
        }
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (!string.IsNullOrEmpty(refreshToken))
            {
                await _authService.RevokeTokenAsync(refreshToken);
            }

            Response.Cookies.Delete("refreshToken");

            return Ok(new AuthMessageDto
            {
                message = "Logged out successfully"
            });
        }
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found");

            await _authService.ChangePasswordAsync(userId, dto);
            return Ok(new AuthMessageDto { message = "Password changed successfully" });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var token = await _authService.ForgotPasswordAsync(dto.Email);
            // Send token via email service
            return Ok(new { message = "Password reset token sent to your email", token });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            await _authService.ResetPasswordAsync(dto);
            return Ok(new AuthMessageDto { message = "Password reset successfully" });
        }
    }
}
