using System.Linq.Expressions;

namespace EvaluationSystem.Infrastructure.Repositories.interfaces
{
    public interface IGenericRepo<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        IQueryable<T> GetAll(bool trackChanges = false);
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false);
        Task AddAsync(T entity);
        void Update(T entity);

        void Delete(T entity);

    }
}
