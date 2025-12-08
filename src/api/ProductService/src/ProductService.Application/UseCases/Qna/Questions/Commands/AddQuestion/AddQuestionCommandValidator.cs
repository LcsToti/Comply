using FluentValidation;

namespace ProductService.Application.UseCases.Qna.Questions.Commands.AddQuestion
{
    public class AddQuestionCommandValidator : AbstractValidator<AddQuestionCommand>
    {
        public AddQuestionCommandValidator()
        {
            RuleFor(p => p.ProductId)
                .NotEmpty().WithMessage("Product ID cannot be empty.");

            RuleFor(p => p.UserId)
                .NotEmpty().WithMessage("User ID cannot be empty.");

            RuleFor(p => p.Text)
                .NotEmpty().WithMessage("Question text cannot be empty.")
                .MaximumLength(500).WithMessage("Question text cannot exceed 500 characters.");
        }
    }
}