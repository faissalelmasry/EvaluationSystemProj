using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluationSystem.Application.DTOs.Reports
{
    public class UserReportDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public double AverageScore { get; set; }
        public int CompletedEvaluations { get; set; }
    }
}
