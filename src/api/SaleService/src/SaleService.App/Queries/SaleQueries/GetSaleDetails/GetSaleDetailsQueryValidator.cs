using FluentValidation;

namespace SalesService.App.Queries.SaleQueries.GetSaleDetails;

public class GetSaleDetailsQueryValidator : AbstractValidator<GetSaleDetailsQuery>
{
    public GetSaleDetailsQueryValidator()
    {
        RuleFor(x => x.SaleId)
            .NotEmpty().WithMessage("SaleId is required");
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");       
    }
}