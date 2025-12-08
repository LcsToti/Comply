using MediatR;

namespace ListingService.App.Commands.AuctionCommands.CompleteNewBid;

public record CompleteNewBidCommand(
    Guid AuctionId,
    Guid BidderId,
    decimal BidValue,
    DateTime ExpiresAt,
    Guid PaymentId): IRequest;
