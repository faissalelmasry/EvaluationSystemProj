using EvaluationSystem.Domain.Models;

namespace EvaluationSystem.Application.interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepo<Department> Departments { get; }
        IRefreshTokenRepository RefreshTokens { get; }
         IGenericRepo<User> Users { get; }
        Task<int> SaveChangesAsync();
    }
}
