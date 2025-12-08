using ListingService.Domain.ListingAggregate;

namespace ListingService.Domain.Repositories;

public interface IListingRepository : IRepository<Listing>
{
    Task<Listing?> GetByProductIdAsync(Guid productId);

    Task<bool> AtomicPrepareBuyNowAsync(Guid listingId);

    Task ReleaseBuyNowLockAsync(Guid listingId);
}
