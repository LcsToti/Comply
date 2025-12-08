using FluentValidation;

namespace Payments.App.UseCases.PaymentCases.CreatePayment
{
    public class CreatePaymentEventValidator : AbstractValidator<CreatePaymentEvent>
    {
        public CreatePaymentEventValidator()
        {
            RuleFor(x => x.SourceId)
                .NotEmpty().WithMessage("SourceId is required.");

            RuleFor(x => x.BuyerId)
                .NotEmpty().WithMessage("BuyerId is required.");

            RuleFor(x => x.ExpiresAt)
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("ExpiresAt must be a future date.");

            RuleFor(x => x.Value)
                .GreaterThan(0)
                .WithMessage("Value must be greater than 0.");

            RuleFor(x => x.PaymentMethod)
                .NotEmpty().WithMessage("PaymentMethod is required.")
                .MaximumLength(50).WithMessage("PaymentMethod cannot exceed 50 characters.");
        }
    }
}
