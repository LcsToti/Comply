using FluentValidation;

namespace SalesService.App.Commands.SaleCommands.Dispute.OpenDispute;

public class OpenDisputeCommandValidator : AbstractValidator<OpenDisputeCommand>
{
    public OpenDisputeCommandValidator()
    {
        RuleFor(x => x.SaleId)
            .NotEmpty().WithMessage("SaleId is required");
        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Reason is required");
    }
}