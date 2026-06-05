using EvaluationSystem.Domain.Models;
using EvaluationSystem.Infrastructure.EntityConfigs;
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
        public DbSet<EvaluationCriteria> EvaluationCriterias { get; set; }
        public DbSet<EvaluationResponse> EvaluationResponses { get; set; }
        public DbSet<EvaluationResult> EvaluationResults { get; set; }
        public DbSet<EvaluationReview> EvaluationReviews { get; set; }
        public DbSet<EvaluationSection> EvaluationSections { get; set; }
        public DbSet<EvaluationTemplate> EvaluationTemplates { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new EvaluationTemplateConfig());
            builder.ApplyConfiguration(new EvaluationSectionConfig());
            builder.ApplyConfiguration(new EvaluationCriteriaConfig());
            builder.ApplyConfiguration(new EvaluationAssignmentConfig());
            builder.ApplyConfiguration(new EvaluationResponseConfig());
            builder.ApplyConfiguration(new EvaluationResultConfig());
            builder.ApplyConfiguration(new EvaluationReviewConfig());
            builder.ApplyConfiguration(new DepartmentConfig());
            builder.ApplyConfiguration(new UserConfig());
            builder.ApplyConfiguration(new RefreshTokenConfig());
            builder.ApplyConfiguration(new RoleConfig());
            
            //Applies on-delete restrict on any foreign key
            foreach (var foreignKey in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                if (foreignKey.DeleteBehavior == DeleteBehavior.Cascade)
                {
                    foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
                }
            }


            // You can add more Fluent API configurations here later if needed
        }
    }
}
