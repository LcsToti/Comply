using ListingService.App.Common.Results;
using ListingService.Contracts.Responses.Auctions;

namespace ListingService.Api.Common.Mappers;

public static class ToBidResponseMapper
{
    public static BidResponse ToBidResponse(this BidResult result)
    {
        return new BidResponse(
            Id: result.Id,
            BidderId: result.BidderId,
            Value: result.Value,
            Status: result.Status,
            BiddedAt: result.BiddedAt,
            OutbiddedAt: result.OutbiddedAt,
            WonAt: result.WonAt);
    }
}
