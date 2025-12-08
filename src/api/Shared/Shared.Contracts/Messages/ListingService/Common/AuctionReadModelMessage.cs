namespace Shared.Contracts.Messages.ListingService.Common;

public record AuctionReadModelMessage(
    Guid AuctionId,
    Guid ListingId,
    List<BidReadModelMessage> Bids,
    string Status,
    AuctionSettingsReadModelMessage Settings,
    bool IsProcessingBid,
    DateTime? EditedAt,
    DateTime? StartedAt,
    DateTime? EndedAt);