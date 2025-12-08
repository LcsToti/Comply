using ListingService.App.Common.Results;
using ListingService.Contracts.Responses.Auctions;

namespace ListingService.Api.Common.Mappers;

public static class ToAuctionResponseMapper
{
    public static AuctionResponse ToAuctionResponse(this AuctionResult result)
    {
        return new AuctionResponse(
          Id: result.Id,
          ListingId: result.ListingId,
          Status: result.Status,
          Settings: result.Settings.ToAuctionSettingsResponse(),
          Bids: [.. result.Bids.Select(b => b!.ToBidResponse())],
          EditedAt: result.EditedAt,
          StartedAt: result.StartedAt,
          EndedAt: result.EndedAt);
    }
}