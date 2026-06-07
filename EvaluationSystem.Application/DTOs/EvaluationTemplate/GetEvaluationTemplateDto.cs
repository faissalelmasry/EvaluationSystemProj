using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvaluationSystem.Application.DTOs.EvaluationSection;

namespace EvaluationSystem.Application.DTOs.EvaluationTemplate
{
    public class GetEvaluationTemplateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<EvaluationSectionDto> Sections { get; set; } = new List<EvaluationSectionDto>();
    }
}
