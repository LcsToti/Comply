using ListingService.Domain.AuctionAggregate.Entities;

namespace ListingService.Domain.Repositories;

public interface IAuctionRepository : IRepository<Auction>
{
    Task<List<Auction>> GetAuctionsByListingIdAsync(Guid productId);

    Task<bool> AtomicPrepareNewBidAsync(Guid auctionId);

    Task ReleaseNewBidLockAsync(Guid auctionId);
}
