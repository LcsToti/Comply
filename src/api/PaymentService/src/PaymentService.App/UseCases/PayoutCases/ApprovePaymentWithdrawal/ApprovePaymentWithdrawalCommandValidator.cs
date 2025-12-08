using FluentValidation;

namespace Payments.App.UseCases.PayoutCases.ApprovePaymentWithdrawal;

public class ApprovePaymentWithdrawalCommandValidator : AbstractValidator<ApprovePaymentWithdrawalCommand>
{
    public ApprovePaymentWithdrawalCommandValidator()
    {
        RuleFor(x => x.PaymentId)
            .NotEmpty().WithMessage("PaymentId is required.");
    }
}