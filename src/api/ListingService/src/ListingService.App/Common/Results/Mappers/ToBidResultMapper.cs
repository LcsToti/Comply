using ListingService.Domain.AuctionAggregate.Entities;

namespace ListingService.App.Common.Results.Mappers;

public static class ToBidResultMapper
{
    public static BidResult ToBidResult(this Bid bid)
    {
        return new BidResult(
            Id: bid.Id,
            BidderId: bid.BidderId,
            Value: bid.Value,
            Status: bid.Status.ToString(),
            BiddedAt: bid.BiddedAt,
            OutbiddedAt: bid.OutbiddedAt,
            WonAt: bid.WonAt);
    }
}
