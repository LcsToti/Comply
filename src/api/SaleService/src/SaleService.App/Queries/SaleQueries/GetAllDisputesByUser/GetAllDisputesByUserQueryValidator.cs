using FluentValidation;

namespace SalesService.App.Queries.SaleQueries.GetAllDisputesByUser;

public class GetAllDisputesByUserQueryValidator : AbstractValidator<GetAllDisputesByUserQuery>
{
    public GetAllDisputesByUserQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");
    }
}