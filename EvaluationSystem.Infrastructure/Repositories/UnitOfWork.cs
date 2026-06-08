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

        public IGenericRepo<EvaluationAssignment> EvaluationAssignments { get; }
        public IGenericRepo<EvaluationResponse> EvaluationResponses { get; }
        public IGenericRepo<EvaluationResult> EvaluationResults { get; }
        public IGenericRepo<EvaluationReview> EvaluationReviews { get; }
        public IGenericRepo<EvaluationCriteria> EvaluationCriterias { get; }
        public IGenericRepo<EvaluationSection> EvaluationSections { get; }
        public IGenericRepo<EvaluationTemplate> EvaluationTemplates { get; }
        public IGenericRepo<User> Users { get; }

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
            EvaluationTemplates = new GenericRepo<EvaluationTemplate>(_context);
            EvaluationSections = new GenericRepo<EvaluationSection>(_context);
            EvaluationCriterias = new GenericRepo<EvaluationCriteria>(_context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
