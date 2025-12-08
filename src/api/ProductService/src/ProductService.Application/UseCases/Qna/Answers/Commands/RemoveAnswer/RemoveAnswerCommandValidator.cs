using FluentValidation;
using FluentValidation.Validators;

namespace ProductService.Application.UseCases.Qna.Answers.Commands.RemoveAnswer;

public class RemoveAnswerCommandValidator : AbstractValidator<RemoveAnswerCommand>
{
    public RemoveAnswerCommandValidator()
    {
        RuleFor(p => p.ProductId).NotNull();
        RuleFor(p => p.SellerId).NotNull();
        RuleFor(p => p.QuestionId).NotNull();

    }
}