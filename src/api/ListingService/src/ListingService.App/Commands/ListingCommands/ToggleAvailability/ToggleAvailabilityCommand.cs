using ListingService.App.Common.Results;
using MediatR;

namespace ListingService.App.Commands.ListingCommands.ToggleAvailability;

public record ToggleAvailabilityCommand(
    Guid UserId,
    Guid ListingId) : IRequest<Result<ListingResult>>;