using ListingService.Domain.AuctionAggregate.Entities;
using Shared.Contracts.Messages.ListingService.Common;

namespace ListingService.App.Services;

public static class AuctionReadModelMapper
{
    public static AuctionReadModelMessage ToAuctionReadModelMessage(this Auction auction)
    {
        var auctionReadModelMessage = new AuctionReadModelMessage(
            AuctionId: auction.Id,
            ListingId: auction.ListingId,
            Bids: [.. auction.Bids.Select(bid => new BidReadModelMessage(
                BidId: bid.Id,
                AuctionId: bid.AuctionId,
                BidderId: bid.BidderId,
                Value: bid.Value,
                Status: bid.Status.ToString(),
                BiddedAt: bid.BiddedAt,
                OutbiddedAt: bid.OutbiddedAt,
                WonAt: bid.WonAt))],
            Status: auction.Status.ToString(),
            Settings: new AuctionSettingsReadModelMessage(
                StartBidValue: auction.Settings.StartBidValue,
                WinBidValue: auction.Settings.WinBidValue,
                StartDate: auction.Settings.StartDate,
                EndDate: auction.Settings.EndDate),
            IsProcessingBid: auction.IsProcessingBid,
            EditedAt: auction.EditedAt,
            StartedAt: auction.StartedAt,
            EndedAt: auction.EndedAt);

        return auctionReadModelMessage;
    }
}
