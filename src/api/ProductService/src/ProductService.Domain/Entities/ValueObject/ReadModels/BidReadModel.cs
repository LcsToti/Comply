namespace ProductService.Domain.Entities.ValueObject.ReadModels;

public record BidReadModel(
    Guid Id,
    Guid AuctionId,
    Guid BidderId,
    decimal Value,
    string Status,
    DateTime BiddedAt,
    DateTime? OutbiddedAt,
    DateTime? WonAt);
