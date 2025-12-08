using ListingService.App.Common.Results;
using MediatR;

namespace ListingService.App.Commands.ListingCommands.PrepareBuyNow;

public record PrepareBuyNowCommand(
    Guid UserId,
    Guid ListingId,
    string PaymentMethod) : IRequest<Result<ListingResult>>;