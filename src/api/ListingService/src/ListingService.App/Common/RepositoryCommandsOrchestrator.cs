using ListingService.App.Common.Interfaces;
using ListingService.Domain.AuctionAggregate.Entities;
using ListingService.Domain.ListingAggregate;
using ListingService.Domain.Repositories;

namespace ListingService.App.Common;

public class RepositoryCommandsOrchestrator(
    IListingRepository listingRepository,
    IAuctionRepository auctionRepository,
    IListingReadModelPublisher listingReadModelPublisher)
{
    private readonly IListingRepository _listingRepository = listingRepository;
    private readonly IAuctionRepository _auctionRepository = auctionRepository;
    private readonly IListingReadModelPublisher _listingReadModelPublisher = listingReadModelPublisher;

    public async Task AddListingAsync(Listing listing, CancellationToken cancellationToken)
    {
        await _listingRepository.AddAsync(listing);
        await _listingReadModelPublisher.PublishAsync(listing.Id, cancellationToken);
    }

    public async Task UpdateListingAsync(Listing listing, CancellationToken cancellationToken)
    {
        await _listingRepository.UpdateAsync(listing);
        await _listingReadModelPublisher.PublishAsync(listing.Id, cancellationToken);
    }

    public async Task AddAuctionAsync(Auction auction, CancellationToken cancellationToken)
    {
        await _auctionRepository.AddAsync(auction);
        await _listingReadModelPublisher.PublishAsync(auction.ListingId, cancellationToken);
    }

    public async Task UpdateAuctionAsync(Auction auction, CancellationToken cancellationToken)
    {
        await _auctionRepository.UpdateAsync(auction);
        await _listingReadModelPublisher.PublishAsync(auction.ListingId, cancellationToken);
    }
}