using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluationSystem.Application.DTOs.EvaluationCriteria
{
    public class CriterionLookupDto
    {
        public int Id { get; set; }
        public int SectionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int OrderNo { get; set; }
    }
}
