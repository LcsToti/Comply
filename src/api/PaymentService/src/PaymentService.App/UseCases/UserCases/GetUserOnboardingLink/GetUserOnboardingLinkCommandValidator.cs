using FluentValidation;

namespace Payments.App.UseCases.UserCases.GetUserOnboardingLink;

public class GetUserOnboardingLinkCommandValidator : AbstractValidator<GetUserOnboardingLinkCommand>
{
    public GetUserOnboardingLinkCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");      
    }
}