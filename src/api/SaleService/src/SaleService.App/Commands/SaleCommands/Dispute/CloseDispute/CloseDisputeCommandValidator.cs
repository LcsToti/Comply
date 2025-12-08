using FluentValidation;

namespace SalesService.App.Commands.SaleCommands.Dispute.CloseDispute;

public class CloseDisputeCommandValidator : AbstractValidator<CloseDisputeCommand>
{
    public CloseDisputeCommandValidator()
    {
        RuleFor(x => x.SaleId)
            .NotEmpty().WithMessage("SaleId is required");
        RuleFor(x => x.Resolution).NotEmpty().WithMessage("Resolution is required");
        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required");
        RuleFor(x => x.ResolutionStatus)
            .IsInEnum().WithMessage("ResolutionStatus is required");

    }
}