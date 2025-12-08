namespace Shared.Contracts.Messages.ListingService.Common;

public record BidReadModelMessage(
    Guid BidId,
    Guid AuctionId,
    Guid BidderId,
    decimal Value,
    string Status,
    DateTime BiddedAt,
    DateTime? OutbiddedAt,
    DateTime? WonAt);