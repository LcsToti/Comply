using FluentValidation;

namespace SalesService.App.Commands.SaleCommands.Sales.CancelSale;

public class CancelSaleCommandValidator : AbstractValidator<CancelSaleCommand>
{
    public CancelSaleCommandValidator()
    {
        RuleFor(x => x.SaleId)
            .NotEmpty().WithMessage("SaleId is required");
        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required");
    }
}