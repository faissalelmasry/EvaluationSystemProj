using EvaluationSystem.Domain.Models;

namespace EvaluationSystem.Application.interfaces
{
    public interface IUnitOfWork
    {
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

        Task<int> SaveChangesAsync();
    }
}
