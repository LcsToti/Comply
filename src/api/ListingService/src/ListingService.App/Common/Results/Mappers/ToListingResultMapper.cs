using ListingService.Domain.ListingAggregate;

namespace ListingService.App.Common.Results.Mappers;

public static class ToListingResultMapper
{
    public static ListingResult ToListingResult(this Listing listing)
    {
        return new ListingResult(
            Id: listing.Id,
            SellerId: listing.SellerId,
            ProductId: listing.ProductId,
            Status: listing.Status.ToString(),
            BuyPrice: listing.BuyPrice,
            IsAuctionActive: listing.IsAuctionActive,
            BuyerId: listing.BuyerId,
            listing.AuctionId,
            listing.ListedAt,
            listing.UpdatedAt);
    }
}