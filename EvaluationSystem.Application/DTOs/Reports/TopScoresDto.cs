using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluationSystem.Application.DTOs.Reports
{
    public class TopScoresDto
    {
        public IEnumerable<EvaluatedScoreDto> TopEvaluatees { get; set; }
        public IEnumerable<EvaluatedScoreDto> LowScoreEvaluations { get; set; }
    }

    public class EvaluatedScoreDto
    {
        public string EvaluateeName { get; set; }
        public double Score { get; set; }
        public string TemplateTitle { get; set; }
    }
}
