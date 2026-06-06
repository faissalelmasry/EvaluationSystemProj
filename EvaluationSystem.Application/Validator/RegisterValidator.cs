using EvaluationSystem.Application.DTOs.Auth;
using FluentValidation;

namespace EvaluationSystem.Application.Validators
{
    public class RegisterValidator : AbstractValidator<RegisterDTO>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();
        }
    }
}