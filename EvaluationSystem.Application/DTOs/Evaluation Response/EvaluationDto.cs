using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluationSystem.Application.DTOs.Evaluation_Response
{
    public class SubmitEvaluationDto
    {
        public List<QuestionResponseDto> Responses { get; set; } = new();
    }

    // 2. This represents each individual answer
    public class QuestionResponseDto
    {
        public int CriterionId { get; set; }

        public decimal Score { get; set; }

        public string? TextAnswer { get; set; }

        public string? SelectedOption { get; set; }

        public string? Comment { get; set; }
    }
        public class EvaluationResponseDto
        {
            public int Id { get; set; } 

            public int AssignmentId { get; set; }

            public int CriterionId { get; set; }

            public string CriterionTitle { get; set; } = string.Empty;

            public decimal Score { get; set; }

            public string? TextAnswer { get; set; }

            public string? SelectedOption { get; set; }

            public string? Comment { get; set; }
        }
    }
