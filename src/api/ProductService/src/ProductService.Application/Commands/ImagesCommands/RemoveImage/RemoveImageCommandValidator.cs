using FluentValidation;

namespace ProductService.Application.Commands.ImagesCommands.RemoveImage;

public class RemoveImageCommandValidator : AbstractValidator<RemoveImageCommand>
{
    public RemoveImageCommandValidator()
    {
        RuleFor(p => p.ProductId).NotEmpty();
        RuleFor(p => p.SellerId).NotEmpty();
        RuleFor(p => p.ImageUrls).NotEmpty().WithMessage("Image URL is required.");
    }
}