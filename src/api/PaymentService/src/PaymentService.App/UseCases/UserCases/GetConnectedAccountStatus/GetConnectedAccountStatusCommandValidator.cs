using FluentValidation;

namespace Payments.App.UseCases.UserCases.GetConnectedAccountStatus;

public class GetConnectedAccountStatusCommandValidator : AbstractValidator<GetConnectedAccountStatusCommand>
{
    public GetConnectedAccountStatusCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");      
    }
}