using FluentValidation;

namespace SalesService.App.Queries.SaleQueries.GetAllSalesByUser;

public class GetMySalesQueryValidator : AbstractValidator<GetMySalesQuery>
{
    public GetMySalesQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");
    }   
}