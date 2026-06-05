namespace EvaluationSystem.Application.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; } // El JWT Asasy
        public DateTime TokenExpiresAt { get; set; } // Haykhlas emta
        public string RefreshToken { get; set; } //key el haygded
        public DateTime RefreshTokenExpiresAt { get; set; }

    }
}
