using MediatR;

namespace ListingService.App.Commands.ListingCommands.AbortBuyNow;

public record AbortBuyNowCommand(
    Guid ListingId,
    DateTime? ExpiresAt = null) : IRequest;