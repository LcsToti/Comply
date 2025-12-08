using FluentValidation;
using Payments.App.UseCases.PayoutCases.WithdrawAllAvailablePayments;

namespace Payments.App.UseCases.WithdrawAllAvailablePayments;

public class WithdrawAllAvailablePaymentsCommandValidator : AbstractValidator<WithdrawAllAvailablePaymentsCommand>
{
    public WithdrawAllAvailablePaymentsCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("SellerId is required.");
    }
}