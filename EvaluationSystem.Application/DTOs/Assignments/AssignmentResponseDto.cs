using EvaluationSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluationSystem.Application.DTOs.Assignments
{
    public class AssignmentResponseDto
    {
        public int Id { get; set; }
        public int TemplateId { get; set; }
        public string TemplateTitle { get; set; }
        public string EvaluatorName { get; set; }
        public string EvaluateeName { get; set; }
        public EvaluationStatus Status { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
