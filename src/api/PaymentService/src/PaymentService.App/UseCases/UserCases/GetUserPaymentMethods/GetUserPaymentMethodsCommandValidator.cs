using FluentValidation;

namespace Payments.App.UseCases.UserCases.GetUserPaymentMethods;

public class GetUserPaymentMethodsCommandValidator : AbstractValidator<GetUserPaymentMethodsCommand>
{
    public GetUserPaymentMethodsCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");      
    }
}