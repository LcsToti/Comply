using FluentValidation;

namespace ProductService.Application.UseCases.Qna.Answers.Commands.AddAnswer
{
    public class AddAnswerToQuestionCommandValidator : AbstractValidator<AddAnswerCommand>
    {
        public AddAnswerToQuestionCommandValidator()
        {
            RuleFor(p => p.ProductId).NotEmpty();
            RuleFor(p => p.QuestionId).NotEmpty();
            RuleFor(p => p.SellerId).NotEmpty();
            RuleFor(p => p.AnswerText)
                .NotEmpty().WithMessage("Answer text cannot be empty.")
                .MaximumLength(1000).WithMessage("Answer text cannot exceed 1000 characters.");
        }
    }
}