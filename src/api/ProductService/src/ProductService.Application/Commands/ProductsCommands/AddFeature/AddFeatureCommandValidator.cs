using FluentValidation;

namespace ProductService.Application.Commands.ProductsCommands.AddFeature;

public class AddFeatureCommandValidator : AbstractValidator<AddFeatureCommand>
{
    public AddFeatureCommandValidator()
    {
        RuleFor(p => p.ProductId).NotEmpty();
        RuleFor(p => p.SellerId).NotEmpty();
        RuleFor(p => p.DurationInDays)
            .GreaterThan(0).WithMessage("Number of days must be positive.");
    }
}