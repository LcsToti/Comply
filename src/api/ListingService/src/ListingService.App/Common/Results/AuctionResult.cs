using ListingService.Domain.AuctionAggregate.ValueObjects;

namespace ListingService.App.Common.Results;

public record AuctionResult(
    Guid Id,
    Guid ListingId,
    List<BidResult> Bids,
    string Status,
    AuctionSettingsResult Settings,
    DateTime? EditedAt,
    DateTime? StartedAt,
    DateTime? EndedAt
);