using EvaluationSystem.Application.DTOs.User;
using FluentValidation;

namespace EvaluationSystem.Application.Validators
{
    public class UpdateUserValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full Name is required.")
                .MaximumLength(100);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email is required.");

            RuleFor(x => x.DepartmentId)
                .GreaterThan(0).WithMessage("Department is required.");

            RuleFor(x => x.JobTitle)
                .IsInEnum().WithMessage("Invalid Job Title.");
        }
    }
}