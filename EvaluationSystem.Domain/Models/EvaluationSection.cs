using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvaluationSystem.Domain.BaseModels;

namespace EvaluationSystem.Domain.Models
{
        public class EvaluationSection : BaseEntity
        {

            public string Title { get; set; } = string.Empty;

            public string? Description { get; set; }

            public int OrderNo { get; set; }
            public int TemplateId { get; set; }
            [ForeignKey(nameof(TemplateId))]
            public EvaluationTemplate Template { get; set; } = null!;

            public ICollection<EvaluationCriteria>? Criteria { get; set; }
        }
}
