using EvaluationSystem.Domain.Models;

namespace EvaluationSystem.Infrastructure.Repositories.interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepo<Department> Departments { get; }

        Task<int> SaveChangesAsync();
    }
}
