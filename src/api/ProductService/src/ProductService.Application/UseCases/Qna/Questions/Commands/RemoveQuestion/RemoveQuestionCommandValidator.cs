using FluentValidation;

namespace ProductService.Application.UseCases.Qna.Questions.Commands.RemoveQuestion;

public class RemoveQuestionCommandValidator : AbstractValidator<RemoveQuestionCommand>
{
    public RemoveQuestionCommandValidator()
    {
        RuleFor(p => p.ProductId).NotEmpty();
        RuleFor(p => p.QuestionId).NotEmpty();
        RuleFor(p => p.UserId).NotEmpty();
    }
}