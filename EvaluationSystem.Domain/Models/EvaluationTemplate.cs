using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvaluationSystem.Domain.BaseModels;

namespace EvaluationSystem.Domain.Models
{
    public class EvaluationTemplate: BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        public int CreatedById { get; set; }
        [ForeignKey(nameof(CreatedById))]
        public virtual User User { get; set; } = null!;
        public ICollection<EvaluationSection>? EvaluationSections { get; set; }

        public ICollection<EvaluationAssignment>? Assignments { get; set; }

    }
}
