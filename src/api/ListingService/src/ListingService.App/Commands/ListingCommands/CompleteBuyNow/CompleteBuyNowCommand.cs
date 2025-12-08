using MediatR;

namespace ListingService.App.Commands.ListingCommands.CompleteBuyNow;

public record CompleteBuyNowCommand(
    Guid ListingId,
    Guid BuyerId,
    DateTime ExpiresAt,
    Guid PaymentId,
    decimal Value) : IRequest;