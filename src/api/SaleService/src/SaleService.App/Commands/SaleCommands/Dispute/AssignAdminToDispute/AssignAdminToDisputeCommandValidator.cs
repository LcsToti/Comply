using FluentValidation;

namespace SalesService.App.Commands.SaleCommands.Dispute.AssignAdminToDispute;

public class AssignAdminToDisputeCommandValidator : AbstractValidator<AssignAdminToDisputeCommand>
{
    public AssignAdminToDisputeCommandValidator()
    {
        RuleFor(x => x.SaleId)
            .NotEmpty().WithMessage("SaleId is required");
        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required");
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");
    }
}