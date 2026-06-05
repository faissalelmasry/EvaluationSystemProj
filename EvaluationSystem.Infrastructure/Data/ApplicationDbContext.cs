using EvaluationSystem.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EvaluationSystem.Infrastructure.Data
{
    public class ApplicationDbContext:IdentityDbContext<User, Role, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Department> Departments { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<EvaluationAssignment> EvaluationAssignments { get; set; }
        public DbSet<EvaluationCriteria> evaluationCriterias { get; set; }
        public DbSet<EvaluationResponse> EvaluationResponses { get; set; }
        public DbSet<EvaluationResult> EvaluationResults { get; set; }
        public DbSet<EvaluationReview> EvaluationReviews { get; set; }
        public DbSet<EvaluationSection> EvaluationSections { get; set; }
        public DbSet<EvaluationTemplate> EvaluationTemplates { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

        
            // You can add more Fluent API configurations here later if needed
        }
    }
}
