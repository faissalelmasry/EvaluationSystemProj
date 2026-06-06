using EvaluationSystem.Application.DTOs.Auth;
using FluentValidation;

namespace EvaluationSystem.Application.Validators
{
    public class LoginValidator : AbstractValidator<LoginDto>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

          
        }
    }
}