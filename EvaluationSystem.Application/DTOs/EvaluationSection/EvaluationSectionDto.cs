using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvaluationSystem.Application.DTOs.EvaluationCriteria;

namespace EvaluationSystem.Application.DTOs.EvaluationSection
{
    public class EvaluationSectionDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<EvaluationCriteriaDto> Criteria { get; set; } = new List<EvaluationCriteriaDto>();
    }
}
