using ListingService.Domain.ListingAggregate;
using ListingService.Infra.Persistence.DataModels;
using Shared.Contracts.Messages.ListingService.ProductReadModel;

namespace ListingService.Infra.Persistence.Mappers;

public static class ListingMapper
{
    public static ListingDataModel ToDataModel(this Listing entity) => new()
    {
        Id = entity.Id,
        SellerId = entity.SellerId,
        ProductId = entity.ProductId,
        Status = entity.Status,
        BuyPrice = entity.BuyPrice,
        IsAuctionActive = entity.IsAuctionActive,
        IsProcessingPurchase = entity.IsProcessingPurchase,
        BuyerId = entity.BuyerId,
        AuctionId = entity.AuctionId,
        ListedAt = entity.ListedAt,
        UpdatedAt = entity.UpdatedAt
    };

    public static Listing ToDomain(this ListingDataModel dm)
    {
        return Listing.Restore(
            dm.Id,
            dm.SellerId,
            dm.ProductId,
            dm.Status,
            dm.BuyPrice,
            dm.IsAuctionActive,
            dm.IsProcessingPurchase,
            dm.BuyerId,
            dm.AuctionId,
            dm.ListedAt,
            dm.UpdatedAt);
    }
}