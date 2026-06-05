using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvaluationSystem.Domain.BaseModels;

namespace EvaluationSystem.Domain.Models
{
    public class EvaluationResponse : BaseEntity
    {
        public decimal Score { get; set; }

        public string? TextAnswer { get; set; }

        public string? SelectedOption { get; set; }

        public string? Comment { get; set; }
        public int AssignmentId { get; set; }

        [ForeignKey(nameof(AssignmentId))]
        public EvaluationAssignment Assignment { get; set; } = null!;
        public int CriterionId { get; set; }

        [ForeignKey(nameof(CriterionId))]
        public EvaluationCriteria Criterion { get; set; } = null!;
    }
}
