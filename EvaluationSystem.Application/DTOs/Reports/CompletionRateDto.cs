using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluationSystem.Application.DTOs.Reports
{
    public class CompletionRateDto
    {
        public int TotalAssignments { get; set; }
        public int CompletedAssignments { get; set; }
        public double RatePercentage { get; set; }
        public IEnumerable<MonthlyTrendDto> MonthlyTrends { get; set; }
    }

    public class MonthlyTrendDto
    {
        public string Month { get; set; } 
        public int TotalCreated { get; set; }
        public int TotalCompleted { get; set; }
    }
}
