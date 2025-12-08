using MediatR;

namespace ListingService.App.Commands.AuctionCommands.AuctionStatusActions.SetAuctionAsEnding;

public record SetAuctionAsEndingCommand(
    Guid AuctionId,
    int Version) : IRequest;
