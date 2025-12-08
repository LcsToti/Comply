namespace ProductService.Domain.Entities.ValueObject.ReadModels;

public record AuctionReadModel(
    Guid Id,
    Guid ListingId,
    List<BidReadModel> Bids,
    string Status,
    AuctionSettingsReadModel Settings,
    bool IsProcessingBid,
    DateTime? EditedAt,
    DateTime? StartedAt,
    DateTime? EndedAt);