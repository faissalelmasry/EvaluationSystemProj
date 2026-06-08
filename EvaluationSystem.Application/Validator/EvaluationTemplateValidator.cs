using EvaluationSystem.Application.DTOs.EvaluationTemplate;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluationSystem.Application.Validator
{
    public class EvaluationTemplateValidator : AbstractValidator<EvaluationTemplateDto>
    {
        public EvaluationTemplateValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Template title is required.")
                .MaximumLength(200).WithMessage("Template title cannot exceed 200 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

            RuleFor(x => x.CreatedById)
                .GreaterThan(0).WithMessage("A valid User ID is required for the creator.");
        }
    }
}