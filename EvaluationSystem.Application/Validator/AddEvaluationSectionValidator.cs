using EvaluationSystem.Application.DTOs.EvaluationSection;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluationSystem.Application.Validator
{
    public class AddEvaluationSectionValidator : AbstractValidator<AddEvaluationSectionDto>
    {
        public AddEvaluationSectionValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Section title is required.")
                .MaximumLength(200).WithMessage("Section title cannot exceed 200 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");
        }
    }
}
