using FluentValidation;

namespace ProductService.Application.UseCases.Qna.Answers.Commands.UpdateAnswer;

public class UpdateAnswerCommandValidator : AbstractValidator<UpdateAnswerCommand>
{
    public UpdateAnswerCommandValidator()
    {
        RuleFor(p => p.SellerId).NotNull();
        RuleFor(p => p.QuestionId).NotNull();
        RuleFor(p => p.ProductId).NotNull();
        RuleFor(p => p.NewAnswerText).NotEmpty().WithMessage("Text cannot be empty.").MaximumLength(500).WithMessage("Text cannot exceed 500 characters.");
    }
}