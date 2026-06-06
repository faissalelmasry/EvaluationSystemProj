using EvaluationSystem.Application.DTOs.Auth;

namespace EvaluationSystem.Application.Services.ServiceInterfaces
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterDTO dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
        Task RevokeTokenAsync(string refreshToken);
    }
}
