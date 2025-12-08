using FluentValidation;

namespace Payments.App.UseCases.RefundCases.CreateRefund
{
    public class CreateRefundEventValidator : AbstractValidator<CreateRefundEvent>
    {
        public CreateRefundEventValidator()
        {
            RuleFor(x => x.PaymentId).NotEmpty().WithMessage("PaymentId is required.");
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be greater than zero.");
            RuleFor(x => x.Reason).NotEmpty().WithMessage("Reason is required.");
        }
    }
}
