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
        public IGenericRepo<User> Users { get; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Departments = new GenericRepo<Department>(_context);
            Users = new GenericRepo<User>(_context);

            RefreshTokens = new RefreshTokenRepository(_context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
