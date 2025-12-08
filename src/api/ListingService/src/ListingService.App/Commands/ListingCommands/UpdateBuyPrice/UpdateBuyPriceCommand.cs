using ListingService.App.Common.Results;
using MediatR;

namespace ListingService.App.Commands.ListingCommands.UpdateBuyPrice;

public record UpdateBuyPriceCommand(
    Guid UserId,
    Guid ListingId,
    decimal NewBuyPrice) : IRequest<Result<ListingResult>>;