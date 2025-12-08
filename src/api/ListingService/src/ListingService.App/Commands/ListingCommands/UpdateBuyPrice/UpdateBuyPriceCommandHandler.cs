using ListingService.App.Common;
using ListingService.App.Common.Errors;
using ListingService.App.Common.Interfaces;
using ListingService.App.Common.Results;
using ListingService.App.Common.Results.Mappers;
using ListingService.Domain.ListingAggregate;
using ListingService.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ListingService.App.Commands.ListingCommands.UpdateBuyPrice;

public class UpdateBuyPriceCommandHandler(
        IListingRepository listingRepository,
        RepositoryCommandsOrchestrator repositoryCommandsOrchestrator,
        IDateTimeProvider dateTimeProvider,
        ILogger<UpdateBuyPriceCommandHandler> logger) : IRequestHandler<UpdateBuyPriceCommand, Result<ListingResult>>
{
    private readonly IListingRepository _listingRepository = listingRepository;
    private readonly RepositoryCommandsOrchestrator _repositoryCommandsOrchestrator = repositoryCommandsOrchestrator;
    private readonly ILogger<UpdateBuyPriceCommandHandler> _logger = logger;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;

    public async Task<Result<ListingResult>> Handle(UpdateBuyPriceCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling UpdateBuyPriceCommand for Listing {ListingId}", request.ListingId);

        // App Logic
        var listing = await _listingRepository.GetByIdAsync(request.ListingId);

        if (listing is null)
            return Result<ListingResult>.Failure(new NotFound(nameof(Listing),request.ListingId));

        if (listing.SellerId != request.UserId)
            return Result<ListingResult>.Failure(new Forbidden("It is not possible to change someone else's listing's buy price."));

        // Domain
        listing.UpdateBuyPrice(request.NewBuyPrice, _dateTimeProvider.UtcNow);

        // Persist
        await _repositoryCommandsOrchestrator.UpdateListingAsync(listing, cancellationToken);

        // Finish
        _logger.LogInformation("Listing {Id} toggled it's visibility", listing.Id);
        return Result<ListingResult>.Success(listing.ToListingResult());
    }
}