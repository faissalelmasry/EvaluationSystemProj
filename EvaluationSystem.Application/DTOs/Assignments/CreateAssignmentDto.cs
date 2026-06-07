using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluationSystem.Application.DTOs.Assignments
{
    public class CreateAssignmentDto
    {
        public int TemplateId { get; set; }
        public int EvaluatorId { get; set; }
        public int EvaluateeId { get; set; }
        public DateTime DueDate { get; set; }
    }
}
