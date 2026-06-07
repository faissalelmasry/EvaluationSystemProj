using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluationSystem.Application.DTOs.Evaluation_Result
{
    public class EvaluationResultDto
    {
        public int AssignmentId { get; set; }

        public decimal TotalScore { get; set; }

        public decimal MaxPossibleScore { get; set; }

        public decimal Percentage { get; set; }

        public string Grade { get; set; } = string.Empty;
    }
}
