using MediatR;
using SalesService.App.Common;
using SalesService.App.Common.Results;

namespace SalesService.App.Commands.SaleCommands.Dispute.OpenDispute;

public record OpenDisputeCommand(Guid SaleId, string Reason, Guid UserId) : IRequest<Result<SaleResult>>;