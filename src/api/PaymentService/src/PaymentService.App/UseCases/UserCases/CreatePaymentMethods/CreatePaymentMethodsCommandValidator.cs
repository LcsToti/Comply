using FluentValidation;

namespace Payments.App.UseCases.UserCases.CreatePaymentMethods;

public class CreatePaymentMethodsCommandValidator : AbstractValidator<CreatePaymentMethodsCommand>
{
    public CreatePaymentMethodsCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");
    }   
}