namespace ListingService.App.Common.Results;

public record BidResult(
    Guid Id,
    Guid BidderId,
    decimal Value,
    string Status,
    DateTime BiddedAt,
    DateTime? OutbiddedAt,
    DateTime? WonAt);