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
    public class EvaluationSectionConfig : IEntityTypeConfiguration<EvaluationSection>
    {
        public void Configure(EntityTypeBuilder<EvaluationSection> builder)
        {
        }
    }
}
