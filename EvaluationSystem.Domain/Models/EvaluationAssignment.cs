using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvaluationSystem.Domain.BaseModels;

namespace EvaluationSystem.Domain.Models
{
    public class EvaluationAssignment : BaseEntity
    {
        public int TemplateId { get; set; }

        public int EvaluatorId { get; set; }

        public int EvaluateeId { get; set; }

        public int AssignedById { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime DueDate { get; set; }

        public DateTime? SubmittedAt { get; set; }

        public EvaluationTemplate Template { get; set; } = null!;

        public User Evaluator { get; set; } = null!;

        public User Evaluatee { get; set; } = null!;

        public User AssignedBy { get; set; } = null!;

        public ICollection<EvaluationResponse>? Responses { get; set; }

        public ICollection<EvaluationReview>? Reviews { get; set; }

        public EvaluationResult? Result { get; set; }
    }
}
