using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvaluationSystem.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvaluationSystem.Infrastructure.EntityConfigs
{
    public class EvaluationReviewConfig : IEntityTypeConfiguration<EvaluationReview>
    {
        public void Configure(EntityTypeBuilder<EvaluationReview> builder)
        {
            builder.Property(r => r.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);
            builder.HasQueryFilter(t => !t.IsDeleted);
        }
    }
}
