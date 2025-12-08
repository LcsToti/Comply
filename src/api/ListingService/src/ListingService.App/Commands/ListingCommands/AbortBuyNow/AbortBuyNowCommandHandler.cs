using DnsClient.Internal;
using ListingService.App.Common;
using ListingService.App.Common.Interfaces;
using ListingService.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ListingService.App.Commands.ListingCommands.AbortBuyNow;

public class AbortBuyNowCommandHandler(
    IListingRepository listingRepository,
    RepositoryCommandsOrchestrator repositoryCommandsOrchestrator,
    IDateTimeProvider dateTimeProvider,
    ILogger<AbortBuyNowCommandHandler> logger) : IRequestHandler<AbortBuyNowCommand>
{
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly IListingRepository _listingRepository = listingRepository;
    private readonly RepositoryCommandsOrchestrator _repositoryCommandsOrchestrator = repositoryCommandsOrchestrator;
    private readonly ILogger<AbortBuyNowCommandHandler> _logger = logger;

    public async Task Handle(AbortBuyNowCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling AbortBuyNowCommand for listing {Id}", request.ListingId);

        var utcNow = _dateTimeProvider.UtcNow;

        if (request.ExpiresAt is not null && utcNow > request.ExpiresAt)
        {
            _logger.LogError("Error while aborting purchase on listing {Id}: Expired", request.ListingId);
            return;
        }

        var listing = await _listingRepository.GetByIdAsync(request.ListingId);
        if (listing is null)
        {
            _logger.LogError("Listing {Id} not found while aborting purchase message", request.ListingId);
            return;
        }

        // Domain
        try
        {
            listing.AbortBuyNow();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while buying listing {Id}", request.ListingId);
            return;
        }

        // Persist
        await _repositoryCommandsOrchestrator.UpdateListingAsync(listing, cancellationToken);
        _logger.LogInformation("Buy Now purchase aborted for listing {Id}", request.ListingId);
    }
}

