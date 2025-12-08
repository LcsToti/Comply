using FluentValidation;

namespace SalesService.App.Commands.SaleCommands.Sales.MarkSaleAsDelivered;

public class MarkSaleAsDeliveredCommandValidator : AbstractValidator<MarkSaleAsDeliveredCommand>
{
    public MarkSaleAsDeliveredCommandValidator()
    {
        RuleFor(x => x.SaleId)
            .NotEmpty().WithMessage("SaleId is required");
        RuleFor(x => x.DeliveryCode)
            .NotEmpty().WithMessage("DeliveryCode is required")
            .Length(6).WithMessage("DeliveryCode must be 6 characters long");       
    }
}