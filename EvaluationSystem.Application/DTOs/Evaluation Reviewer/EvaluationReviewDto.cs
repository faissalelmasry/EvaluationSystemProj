using EvaluationSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluationSystem.Application.DTOs.Evaluation_Reviewer
{
    public class SubmitReviewDto
    {
        public string? ReviewComment { get; set; }
    }
    public class EvaluationReviewDto
    {
        public int Id { get; set; }

        public int AssignmentId { get; set; }

        public int ReviewerId { get; set; }

        public string? ReviewComment { get; set; }

        public ReviewStatus Status { get; set; }

        public DateTime ReviewedAt { get; set; }
    }
}
