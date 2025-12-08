using FluentValidation;

namespace ProductService.Application.Commands.ProductsCommands.UpdateProduct
{
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(p => p.ProductId).NotEmpty();
            RuleFor(p => p.SellerId).NotEmpty();

            RuleFor(p => p.Title)
                .MaximumLength(100).WithMessage("Title must not exceed 100 characters.")
                .When(p => p.Title != null);

            RuleFor(p => p.Description).MaximumLength(3000).WithMessage("Description must not exceed 3000 characters.");

            RuleFor(p => p.Characteristics)
                .Must(c => c!.Count >= 1 && c.Count <= 50)
                .WithMessage("Product must have between 1 and 50 characteristics.")
                .When(p => p.Characteristics != null);
        }
    }
}