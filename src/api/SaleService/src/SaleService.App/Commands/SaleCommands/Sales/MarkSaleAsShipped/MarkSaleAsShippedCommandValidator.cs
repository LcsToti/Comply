using FluentValidation;

namespace SalesService.App.Commands.SaleCommands.Sales.MarkSaleAsShipped;

public class MarkSaleAsShippedCommandValidator : AbstractValidator<MarkSaleAsShippedCommand>
{
    public MarkSaleAsShippedCommandValidator()
    {
        RuleFor(x => x.SaleId)
            .NotEmpty().WithMessage("SaleId is required");
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");
    }
}