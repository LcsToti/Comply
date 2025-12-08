using FluentValidation;

namespace Payments.App.UseCases.PaymentCases.GetUserPayments
{
    public class GetPayerPaymentsCommandValidator : AbstractValidator<GetPayerPaymentsCommand>
    {
        public GetPayerPaymentsCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");
        }
    }
}
