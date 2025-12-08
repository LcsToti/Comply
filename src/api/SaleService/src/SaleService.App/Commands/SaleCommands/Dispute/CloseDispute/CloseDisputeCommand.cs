using MediatR;
using SalesService.App.Common;
using SalesService.App.Common.Results;
using SalesService.Domain.Aggregates.SaleAggregate.Enums;

namespace SalesService.App.Commands.SaleCommands.Dispute.CloseDispute;

public record CloseDisputeCommand(Guid SaleId, string Resolution, string Role, DisputeResolutionStatus ResolutionStatus, Guid UserId) : IRequest<Result<SaleResult>>;