namespace ListingService.Contracts.Responses.Auctions;

public record AuctionResponse(
    Guid Id,
    Guid ListingId,
    string Status,
    AuctionSettingsResponse Settings,
    List<BidResponse> Bids,
    DateTime? EditedAt,
    DateTime? StartedAt,
    DateTime? EndedAt);