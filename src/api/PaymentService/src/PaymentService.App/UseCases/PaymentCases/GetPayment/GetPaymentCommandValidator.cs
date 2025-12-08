using FluentValidation;

namespace Payments.App.UseCases.PaymentCases.GetPayment
{
    public class GetPaymentCommandValidator : AbstractValidator<GetPaymentCommand>
    {
        public GetPaymentCommandValidator()
        {
            RuleFor(x => x.PaymentId)
                .NotEmpty().WithMessage("PaymentId is required.");
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("PayerId is required.");       
        }
    }
}
