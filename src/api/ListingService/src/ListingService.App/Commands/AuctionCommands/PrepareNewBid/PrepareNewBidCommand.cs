using ListingService.App.Common.Results;
using MediatR;

namespace ListingService.App.Commands.AuctionCommands.PrepareNewBid;

public record PrepareNewBidCommand(
    Guid UserId,
    Guid AuctionId,
    decimal BidValue,
    string PaymentMethod) : IRequest<Result<AuctionResult>>;