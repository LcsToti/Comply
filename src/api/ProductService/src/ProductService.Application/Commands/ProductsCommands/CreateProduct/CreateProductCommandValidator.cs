using FluentValidation;

namespace ProductService.Application.Commands.ProductsCommands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(p => p.SellerId)
            .NotEmpty().WithMessage("SellerId is required.");

        RuleFor(p => p.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");

        RuleFor(p => p.Description).NotEmpty().WithMessage("Description is required.").MaximumLength(3000).WithMessage("Description must not exceed 3000 characters.");

        RuleFor(p => p.Locale)
            .NotEmpty().WithMessage("Locale is required.");

        RuleFor(p => p.Characteristics).NotEmpty().WithMessage("Characteristics are required.");

        RuleFor(p => p.Condition)                              
            .NotEmpty().WithMessage("Condition is required.");
                
        RuleFor(p => p.Category).NotEmpty().WithMessage("Category is required.");

        RuleFor(p => p.DeliveryPreference).NotEmpty().WithMessage("DeliveryPreferences are required.");
    }
}