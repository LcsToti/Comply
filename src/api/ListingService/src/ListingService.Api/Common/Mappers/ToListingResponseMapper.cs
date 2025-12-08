using ListingService.App.Common.Results;
using ListingService.Contracts.Responses;

namespace ListingService.Api.Common.Mappers;

public static class ToListingResponseMapper
{
    public static ListingResponse ToListingResponse(this ListingResult result)
    {
        return new ListingResponse(
            Id: result.Id,
            SellerId: result.SellerId,
            ProductId: result.ProductId,
            Status: result.Status,
            BuyPrice: result.BuyPrice,
            IsAuctionActive: result.IsAuctionActive,
            BuyerId: result.BuyerId,
            AuctionId: result.AuctionId,
            ListedAt: result.ListedAt,
            UpdatedAt: result.UpdatedAt);
    }
}