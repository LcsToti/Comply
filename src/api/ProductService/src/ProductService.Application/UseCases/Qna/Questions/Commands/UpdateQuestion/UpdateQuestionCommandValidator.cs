using FluentValidation;

namespace ProductService.Application.UseCases.Qna.Questions.Commands.UpdateQuestion;

public class UpdateQuestionCommandValidator : AbstractValidator<UpdateQuestionCommand>
{
    public UpdateQuestionCommandValidator()
    {
        RuleFor(p => p.ProductId).NotEmpty();
        RuleFor(p => p.QuestionId).NotEmpty();
        RuleFor(p => p.UserId).NotEmpty();
        RuleFor(p => p.Text)
            .NotEmpty().WithMessage("Question text cannot be empty.")
            .MaximumLength(500).WithMessage("Question text cannot exceed 500 characters.");
    }
    
}