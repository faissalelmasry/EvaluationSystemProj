using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluationSystem.Application.DTOs.Reports
{
    public class DashboardSummaryDto
    {
        public int TotalUsers { get; set; }
        public int TotalTemplates { get; set; }
        public int PendingEvaluations { get; set; }
        public int CompletedEvaluations { get; set; }
        public double AverageScore { get; set; }
        public IEnumerable<string> RecentActivity { get; set; }
    }
}
