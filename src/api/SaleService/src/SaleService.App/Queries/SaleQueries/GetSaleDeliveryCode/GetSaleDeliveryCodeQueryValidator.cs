using FluentValidation;

namespace SalesService.App.Queries.SaleQueries.GetSaleDeliveryCode;

public class GetSaleDeliveryCodeQueryValidator : AbstractValidator<GetSaleDeliveryCodeQuery>
{
    public GetSaleDeliveryCodeQueryValidator()
    {
        RuleFor(x => x.SaleId)
            .NotEmpty().WithMessage("SaleId is required");       
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");       
    }
}