using ListingService.App.Common.Results;
using MediatR;

namespace ListingService.App.Commands.AuctionCommands.Cancel;

public record CancelAuctionCommand(
    Guid UserId,
    Guid AuctionId) : IRequest<Result<AuctionResult>>;