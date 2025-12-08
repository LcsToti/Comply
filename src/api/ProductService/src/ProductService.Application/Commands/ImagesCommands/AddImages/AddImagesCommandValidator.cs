using FluentValidation;

namespace ProductService.Application.Commands.ImagesCommands.AddImages;

public class AddImagesCommandValidator : AbstractValidator<AddImagesCommand>
{
    public AddImagesCommandValidator()
    {
        RuleFor(p => p.ProductId).NotEmpty();
        RuleFor(p => p.SellerId).NotEmpty();
        RuleFor(p => p.ImageUrls)
            .NotEmpty().WithMessage("At least one image URL is required.");
    }
}