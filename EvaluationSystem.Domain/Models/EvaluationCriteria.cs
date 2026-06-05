using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvaluationSystem.Domain.BaseModels;
using EvaluationSystem.Domain.Enums;

namespace EvaluationSystem.Domain.Models
{
    public class EvaluationCriteria:BaseEntity
    {

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public QuestionType QuestionType { get; set; } 

        public decimal MaxScore { get; set; }

        public decimal Weight { get; set; }

        public bool IsRequired { get; set; }

        public int OrderNo { get; set; }
        public int SectionId { get; set; }

        [ForeignKey(nameof(SectionId))]
        public EvaluationSection Section { get; set; } = null!;

        public ICollection<EvaluationResponse>? Responses { get; set; }
    }
}
