using ListingService.App.Common.Results;
using MediatR;

namespace ListingService.App.Commands.ListingCommands.Create;

public record CreateListingCommand(
    Guid UserId,
    Guid ProductId,
    decimal BuyPrice) : IRequest<Result<ListingResult>>;