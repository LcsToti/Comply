using MediatR;

namespace ListingService.App.Commands.AuctionCommands.AuctionStatusActions.FinishAuction;

public record FinishAuctionCommand(
    Guid AuctionId,
    int Version) : IRequest;
