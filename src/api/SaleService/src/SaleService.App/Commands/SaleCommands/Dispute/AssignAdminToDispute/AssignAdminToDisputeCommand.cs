using MediatR;
using SalesService.App.Common;
using SalesService.App.Common.Results;

namespace SalesService.App.Commands.SaleCommands.Dispute.AssignAdminToDispute;

public record AssignAdminToDisputeCommand(Guid SaleId, string Role, Guid UserId): IRequest<Result<SaleResult>>;