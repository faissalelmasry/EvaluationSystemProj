using EvaluationSystem.Domain.Models;

namespace EvaluationSystem.Application.interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepo<Department> Departments { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        IGenericRepo<EvaluationAssignment> EvaluationAssignments { get; }
        IGenericRepo<EvaluationResponse> EvaluationResponses { get; }
        IGenericRepo<EvaluationResult> EvaluationResults { get; }
        IGenericRepo<EvaluationReview> EvaluationReviews { get; }

         IGenericRepo<User> Users { get; }
        Task<int> SaveChangesAsync();
    }
}
