using EvaluationSystem.Application.DTOs.EvaluationCriteria;
using FluentValidation;

public class AddEvaluationCriteriaDtoValidator
    : AbstractValidator<AddEvaluationCriteriaDto>
{
    public AddEvaluationCriteriaDtoValidator()
    {
        RuleFor(x => x.Title)
              .NotEmpty()
              .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(1000);

        RuleFor(x => x.QuestionType)
            .IsInEnum();

        RuleFor(x => x.MaxScore)
            .GreaterThan(0);

        RuleFor(x => x.Weight)
            .GreaterThan(0);

        RuleFor(x => x.OrderNo)
            .GreaterThan(0);
    }
}