using EvaluationSystem.Application.DTOs.Evaluation_Reviewer;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluationSystem.Application.Validator
{
    public class SubmitReviewValidator:AbstractValidator<SubmitReviewDto>
    {
        public SubmitReviewValidator()
        {
            RuleFor(x => x.ReviewComment)
                            .MaximumLength(500).WithMessage("Review comment cannot exceed 500 characters.");
        }
    }
}
