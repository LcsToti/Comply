using ListingService.Domain.AuctionAggregate.ValueObjects;

namespace ListingService.App.Common.Results.Mappers;

public static class ToAuctionSettingsResultMapper
{
    public static AuctionSettingsResult ToAuctionSettingsResult(this AuctionSettings auctionSettings)
    {
        return new AuctionSettingsResult(
            StartBidValue: auctionSettings.StartBidValue,
            WinBidValue: auctionSettings.WinBidValue,
            StartDate: auctionSettings.StartDate,
            EndDate: auctionSettings.EndDate);
    }
}