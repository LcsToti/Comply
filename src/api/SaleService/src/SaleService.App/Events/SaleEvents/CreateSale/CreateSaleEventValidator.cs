using FluentValidation;

namespace SalesService.App.Events.SaleEvents.CreateSale;

public class CreateSaleEventValidator : AbstractValidator<CreateSaleEvent>
{
    public CreateSaleEventValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("ProductId is required");
        RuleFor(x => x.BuyerId)
            .NotEmpty().WithMessage("BuyerId is required");
        RuleFor(x => x.SellerId)
            .NotEmpty().WithMessage("SellerId is required");
        RuleFor(x => x.ListingId)
            .NotEmpty().WithMessage("ListingId is required");
        RuleFor(x => x.PaymentId)
            .NotEmpty().WithMessage("PaymentId is required");
        RuleFor(x => x.ProductValue)
            .NotEmpty().WithMessage("ProductValue is required")
            .LessThan(0).WithMessage("ProductValue must be greater than 0");
    }
}