using EvaluationSystem.Application.DTOs.Assignments;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluationSystem.Application.Validator
{
    public class CreateAssignmentValidator : AbstractValidator<CreateAssignmentDto>
    {
        public CreateAssignmentValidator()
        {
            RuleFor(x => x.TemplateId)
                .GreaterThan(0).WithMessage("A valid Template ID is required.");

            RuleFor(x => x.EvaluatorId)
                .GreaterThan(0).WithMessage("A valid Evaluator ID is required.");

            RuleFor(x => x.EvaluateeId)
                .GreaterThan(0).WithMessage("A valid Evaluatee ID is required.")
                .NotEqual(x => x.EvaluatorId).WithMessage("The evaluator and evaluatee cannot be the same person.");

            RuleFor(x => x.DueDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("The due date must be set in the future.");
        }
    }
}
