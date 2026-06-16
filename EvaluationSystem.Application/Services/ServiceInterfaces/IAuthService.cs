using EvaluationSystem.Application.DTOs.Auth;

namespace EvaluationSystem.Application.Services.ServiceInterfaces
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterDTO dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
        Task RevokeTokenAsync(string refreshToken);
        Task ChangePasswordAsync(string userId, ChangePasswordDto dto);
        Task<string> ForgotPasswordAsync(string email);
        Task ResetPasswordAsync(ResetPasswordDto dto);
    }
}
