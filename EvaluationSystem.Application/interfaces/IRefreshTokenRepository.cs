using EvaluationSystem.Domain.Models;

namespace EvaluationSystem.Application.interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token);

        Task AddAsync(RefreshToken refreshToken);

        void Update(RefreshToken refreshToken);
    }
}