using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvaluationSystem.Application.DTOs.EvaluationCriteria;

namespace EvaluationSystem.Application.DTOs.EvaluationSection
{
    public class AddTemplateSectionDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int OrderNo { get; set; }
        public List<AddTemplateCriteriaDto> Criteria { get; set; } = new List<AddTemplateCriteriaDto>();
    }
}
