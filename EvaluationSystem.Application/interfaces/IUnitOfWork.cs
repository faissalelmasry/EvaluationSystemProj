using EvaluationSystem.Domain.Models;

namespace EvaluationSystem.Application.interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepo<Department> Departments { get; }

        Task<int> SaveChangesAsync();
    }
}
