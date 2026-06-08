using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvaluationSystem.Domain.Enums;

namespace EvaluationSystem.Application.DTOs.EvaluationCriteria
{
    public class AddEvaluationCriteriaDto
    {
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public QuestionType QuestionType { get; set; }

        public decimal MaxScore { get; set; }

        public decimal Weight { get; set; }

        public bool IsRequired { get; set; }

        public int OrderNo { get; set; }
    }
}
