using FluentValidation;

namespace Payments.App.UseCases.UserCases.GetUserDashboardLink;

public class GetUserDashboardLinkCommandValidator : AbstractValidator<GetUserDashboardLinkCommand>
{
    public GetUserDashboardLinkCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");      
    }
}