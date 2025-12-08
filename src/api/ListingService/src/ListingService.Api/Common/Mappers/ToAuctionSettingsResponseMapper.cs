using ListingService.App.Common.Results;
using ListingService.Contracts.Responses.Auctions;

namespace ListingService.Api.Common.Mappers;

public static class ToAuctionSettingsResponseMapper
{
    public static AuctionSettingsResponse ToAuctionSettingsResponse(this AuctionSettingsResult result)
    {
        return new AuctionSettingsResponse(
            StartBidValue: result.StartBidValue,
            WinBidValue: result.WinBidValue,
            StartDate: result.StartDate,
            EndDate: result.EndDate);
    }
}
