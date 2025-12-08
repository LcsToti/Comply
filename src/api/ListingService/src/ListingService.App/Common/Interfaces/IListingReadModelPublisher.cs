namespace ListingService.App.Common.Interfaces;

public interface IListingReadModelPublisher
{
    Task PublishAsync(Guid listingId, CancellationToken cancellationToken);
}