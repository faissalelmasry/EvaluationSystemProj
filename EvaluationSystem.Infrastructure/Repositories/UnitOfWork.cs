using EvaluationSystem.Infrastructure.Data;
using EvaluationSystem.Domain.Models;
using EvaluationSystem.Application.interfaces;

namespace EvaluationSystem.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IGenericRepo<Department> Departments { get; }

        public IRefreshTokenRepository RefreshTokens { get; }


        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Departments = new GenericRepo<Department>(_context);

            RefreshTokens = new RefreshTokenRepository(_context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public async Task ExecuteInTransactionAsync(Func<Task> operation)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await operation();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
