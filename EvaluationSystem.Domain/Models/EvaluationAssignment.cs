using EvaluationSystem.Domain.BaseModels;
using EvaluationSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluationSystem.Domain.Models
{
    public class EvaluationAssignment : BaseEntity
    {
        public int TemplateId { get; set; }

        public int EvaluatorId { get; set; }

        public int EvaluateeId { get; set; }

        public int AssignedById { get; set; }

        public EvaluationStatus Status { get; set; } = EvaluationStatus.Pending;

        public DateTime DueDate { get; set; }

        public DateTime? SubmittedAt { get; set; }

        [ForeignKey(nameof(TemplateId))]
        public EvaluationTemplate Template { get; set; } = null!;

        [ForeignKey(nameof(EvaluatorId))]
        public User Evaluator { get; set; } = null!;

        [ForeignKey(nameof(EvaluateeId))]
        public User Evaluatee { get; set; } = null!;
        [ForeignKey(nameof(AssignedById))]

        public User AssignedBy { get; set; } = null!;

        public ICollection<EvaluationResponse>? Responses { get; set; }

        public ICollection<EvaluationReview>? Reviews { get; set; }

        public EvaluationResult? Result { get; set; }
    }
}
