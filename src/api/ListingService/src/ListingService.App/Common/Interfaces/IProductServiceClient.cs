namespace ListingService.App.Common.Interfaces;

public interface IProductServiceClient
{
    Task<Guid?> GetSellerIdByProductIdAsync(Guid productId, CancellationToken cancellationToken);
}