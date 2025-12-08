using ListingService.App.Common.Results;
using MediatR;

namespace ListingService.App.Commands.AuctionCommands.EditSettings;

public record EditSettingsCommand(
    Guid UserId,
    Guid AuctionId,
    decimal? StartBidValue,
    decimal? WinBidValue,
    DateTime? StartDate,
    DateTime? EndDate) : IRequest<Result<AuctionResult>>;