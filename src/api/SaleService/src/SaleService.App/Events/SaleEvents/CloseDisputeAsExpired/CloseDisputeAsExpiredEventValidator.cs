using FluentValidation;

namespace SalesService.App.Events.SaleEvents.CloseDisputeAsExpired;

public class CloseDisputeAsExpiredEventValidator : AbstractValidator<CloseDisputeAsExpiredEvent>
{
    public CloseDisputeAsExpiredEventValidator()
    {
        RuleFor(x => x.SaleId)
            .NotEmpty().WithMessage("SaleId is required");
        RuleFor(x => x.ExpiresAt)
            .NotEmpty().WithMessage("ExpiresAt is required");
    }
}