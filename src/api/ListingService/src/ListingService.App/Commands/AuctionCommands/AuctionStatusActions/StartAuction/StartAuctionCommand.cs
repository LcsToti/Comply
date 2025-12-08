using MediatR;

namespace ListingService.App.Commands.AuctionCommands.AuctionStatusActions.StartAuction;

public record StartAuctionCommand(
    Guid AuctionId,
    int Version) : IRequest;