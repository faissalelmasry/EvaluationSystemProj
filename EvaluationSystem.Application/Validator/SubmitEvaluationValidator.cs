using EvaluationSystem.Application.DTOs.Evaluation_Response;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluationSystem.Application.Validator
{
    public class SubmitEvaluationValidator:AbstractValidator<SubmitEvaluationDto>
    {
        public SubmitEvaluationValidator()
        {
            RuleFor(x => x.Responses)
                .NotNull().WithMessage("The evaluation responses cannot be null.")
                .NotEmpty().WithMessage("You must submit at least one response.");

            RuleForEach(x => x.Responses).SetValidator(new QuestionResponseValidator());
        }
    }
}
