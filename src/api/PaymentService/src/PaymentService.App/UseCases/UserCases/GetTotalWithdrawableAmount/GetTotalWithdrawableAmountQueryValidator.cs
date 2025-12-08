using FluentValidation;

namespace Payments.App.UseCases.UserCases.GetTotalWithdrawableAmount;

public class GetTotalWithdrawableAmountQueryValidator : AbstractValidator<GetTotalWithdrawableAmountCommand>
{
    public GetTotalWithdrawableAmountQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}