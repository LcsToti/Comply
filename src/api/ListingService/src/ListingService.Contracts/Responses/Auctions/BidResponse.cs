namespace ListingService.Contracts.Responses.Auctions;

public record BidResponse(
    Guid Id,
    Guid BidderId,
    decimal Value,
    string Status,
    DateTime BiddedAt,
    DateTime? OutbiddedAt,
    DateTime? WonAt);
