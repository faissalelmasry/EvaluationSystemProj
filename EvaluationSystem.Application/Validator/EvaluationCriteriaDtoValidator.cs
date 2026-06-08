using EvaluationSystem.Application.DTOs.EvaluationCriteria;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluationSystem.Application.Validator
{
    public class EvaluationCriteriaDtoValidator : AbstractValidator<EvaluationCriteriaDto>
    {
        public EvaluationCriteriaDtoValidator()
        {
            RuleFor(x => x.Title)
              .NotEmpty()
              .MaximumLength(200);

            RuleFor(x => x.Description)
                .MaximumLength(1000);

            RuleFor(x => x.MaxScore)
                .GreaterThan(0);
        }
    }
}
