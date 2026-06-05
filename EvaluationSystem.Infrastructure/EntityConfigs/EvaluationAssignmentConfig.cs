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
    public class EvaluationAssignmentConfig : IEntityTypeConfiguration<EvaluationAssignment>
    {
        public void Configure(EntityTypeBuilder<EvaluationAssignment> builder)
        {
            builder.Property(a => a.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);
        }
    }
}
