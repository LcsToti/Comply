using MediatR;

namespace SalesService.App.Events.SaleEvents.CloseDisputeAsExpired;

public record CloseDisputeAsExpiredEvent(Guid SaleId, TimeSpan ExpiresAt) : IRequest;