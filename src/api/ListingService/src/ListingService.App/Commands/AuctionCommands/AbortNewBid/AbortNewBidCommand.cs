using MediatR;

namespace ListingService.App.Commands.AuctionCommands.AbortNewBid;

public record AbortNewBidCommand(
    Guid AuctionId,
    DateTime? ExpiresAt = null) : IRequest;