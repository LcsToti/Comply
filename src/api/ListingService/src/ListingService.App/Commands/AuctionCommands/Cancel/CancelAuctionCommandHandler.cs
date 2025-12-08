using ListingService.App.Common;
using ListingService.App.Common.Errors;
using ListingService.App.Common.Interfaces;
using ListingService.App.Common.Results;
using ListingService.App.Common.Results.Mappers;
using ListingService.Domain.AuctionAggregate.Entities;
using ListingService.Domain.ListingAggregate;
using ListingService.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ListingService.App.Commands.AuctionCommands.Cancel;

public class CancelAuctionCommandHandler(
    IAuctionRepository auctionRepository,
    IListingRepository listingRepository,
    RepositoryCommandsOrchestrator repositoryCommandsOrchestrator,
    IDateTimeProvider dateTimeProvider,
    ILogger<CancelAuctionCommandHandler> logger) : IRequestHandler<CancelAuctionCommand, Result<AuctionResult>>
{
    private readonly IAuctionRepository _auctionRepository = auctionRepository;
    private readonly IListingRepository _listingRepository = listingRepository;
    private readonly RepositoryCommandsOrchestrator _repositoryCommandsOrchestrator = repositoryCommandsOrchestrator;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly ILogger<CancelAuctionCommandHandler> _logger = logger;

    public async Task<Result<AuctionResult>> Handle(CancelAuctionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling CancelAuctionCommand for Auction {AuctionId}.", request.AuctionId);

        // App Logic
        var auction = await _auctionRepository.GetByIdAsync(request.AuctionId);
        if (auction is null)
            return Result<AuctionResult>.Failure(new NotFound(nameof(Auction), request.AuctionId));

        var listing = await _listingRepository.GetByIdAsync(auction.ListingId);

        if (listing is null)
            return Result<AuctionResult>.Failure(new NotFound(nameof(Listing), request.AuctionId));
        
        if (listing.SellerId != request.UserId)
            return Result<AuctionResult>.Failure(new Forbidden("It is not possible to cancel someone else's auction."));
       
        // Domain
        auction.Cancel(_dateTimeProvider.UtcNow);

        // Domain Effects
        listing.SetAuctionInactive(_dateTimeProvider.UtcNow); // Before detaching!
        listing.DetachAuction(_dateTimeProvider.UtcNow);

        // Persist
        await _repositoryCommandsOrchestrator.UpdateAuctionAsync(auction, cancellationToken);
        await _repositoryCommandsOrchestrator.UpdateListingAsync(listing, cancellationToken);

        // Finish
        _logger.LogInformation("Auction {AuctionId} was cancelled successfully", auction.Id);
        var result = auction.ToAuctionResult();
        return Result<AuctionResult>.Success(auction.ToAuctionResult());
    }
}