using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvaluationSystem.Domain.Enums;

namespace EvaluationSystem.Application.DTOs.EvaluationCriteria
{
    public class EvaluationCriteriaDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int OrderNo { get; set; }
        public QuestionType QuestionType { get; set; }
        public int MaxScore { get; set; }
        public int Weight { get; set; }
        public bool IsRequired { get; set; }
    }
}
