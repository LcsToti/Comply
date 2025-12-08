using ListingService.App.Common.Results;
using MediatR;

namespace ListingService.App.Commands.AuctionCommands.Create;

public record CreateAuctionCommand(
    Guid UserId,
    Guid ListingId,
    decimal StartBidValue,
    decimal WinBidValue,
    DateTime StartDate,
    DateTime EndDate) : IRequest<Result<AuctionResult>>;