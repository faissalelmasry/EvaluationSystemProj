using EvaluationSystem.Domain.BaseModels;
using EvaluationSystem.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EvaluationSystem.Domain.Models
{
    public class EvaluationReview : BaseEntity
    {
        public string? ReviewComment { get; set; }

        public DateTime ReviewedAt { get; set; } = DateTime.UtcNow;

        public ReviewStatus Status { get; set; } = ReviewStatus.Pending;

        public int AssignmentId { get; set; }

        public int ReviewerId { get; set; }

        [ForeignKey(nameof(AssignmentId))]
        public EvaluationAssignment Assignment { get; set; } = null!;

        [ForeignKey(nameof(ReviewerId))]
        public User Reviewer { get; set; } = null!;
    }
}