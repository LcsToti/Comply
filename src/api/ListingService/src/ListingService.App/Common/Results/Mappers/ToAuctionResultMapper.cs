using ListingService.Domain.AuctionAggregate.Entities;

namespace ListingService.App.Common.Results.Mappers;

public static class ToAuctionResultMapper
{
    public static AuctionResult ToAuctionResult(this Auction auction)
    {
        return new AuctionResult(
            Id: auction.Id,
            ListingId: auction.ListingId,
            Bids: [.. auction.Bids.Select(b => b!.ToBidResult())],
            Status: auction.Status.ToString(),
            Settings: auction.Settings.ToAuctionSettingsResult(),
            EditedAt: auction.EditedAt,
            StartedAt: auction.StartedAt,
            EndedAt: auction.EndedAt);
    }
}