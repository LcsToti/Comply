using FluentValidation;

namespace Payments.App.UseCases.PayoutCases.WithdrawPayment;

public class WithdrawPaymentCommandValidator : AbstractValidator<WithdrawPaymentCommand>
{
    public WithdrawPaymentCommandValidator()
    {
        RuleFor(x => x.PaymentId)
            .NotEmpty().WithMessage("PaymentId is required.");
    }
}