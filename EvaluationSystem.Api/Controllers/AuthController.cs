using EvaluationSystem.Application.Services;
using EvaluationSystem.Application.DTOs.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            return Ok(authResponse);
        }
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto model)
        {
           
            var response = await _authService.RefreshTokenAsync(model.Token);
            return Ok(response);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RevokeTokenDto model)
        {
            await _authService.RevokeTokenAsync(model.Token);
            return Ok(new AuthMessageDto { message = "Logged out successfully. Token revoked." });
        }
    }
}
