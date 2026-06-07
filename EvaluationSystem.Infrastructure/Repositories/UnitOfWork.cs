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
            Users = new GenericRepo<User>(_context);

            RefreshTokens = new RefreshTokenRepository(_context);
            EvaluationAssignments = new GenericRepo<EvaluationAssignment>(_context);
            EvaluationResponses = new GenericRepo<EvaluationResponse>(_context);
            EvaluationResults = new GenericRepo<EvaluationResult>(_context);
            EvaluationReviews = new GenericRepo<EvaluationReview>(_context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
