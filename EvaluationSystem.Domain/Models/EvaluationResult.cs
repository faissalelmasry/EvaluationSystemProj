using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvaluationSystem.Domain.BaseModels;

namespace EvaluationSystem.Domain.Models
{
    public class EvaluationResult : BaseEntity
    {

        public decimal TotalScore { get; set; }

        public decimal MaxPossibleScore { get; set; }

        public decimal Percentage { get; set; }

        public string Grade { get; set; } = string.Empty;
        public int AssignmentId { get; set; }

        [ForeignKey(nameof(AssignmentId))]
        public EvaluationAssignment Assignment { get; set; } = null!;
    }
}
