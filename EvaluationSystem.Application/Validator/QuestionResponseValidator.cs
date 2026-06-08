using EvaluationSystem.Application.DTOs.Evaluation_Response;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluationSystem.Application.Validator
{
    internal class QuestionResponseValidator:AbstractValidator<QuestionResponseDto>
    {
        public QuestionResponseValidator() {
            RuleFor(x => x.CriterionId)
                .GreaterThan(0).WithMessage("A valid Criterion ID is required.");
            RuleFor(x => x.Score)
                .GreaterThanOrEqualTo(0).WithMessage("Score cannot be negative.");
            RuleFor(x => x.TextAnswer)
                .MaximumLength(1000).WithMessage("Text answer cannot exceed 1000 characters.");

            RuleFor(x => x.SelectedOption)
                .MaximumLength(100).WithMessage("Selected option cannot exceed 100 characters.");

            RuleFor(x => x.Comment)
                .MaximumLength(500).WithMessage("Comment cannot exceed 500 characters.");
        }
    }
}
