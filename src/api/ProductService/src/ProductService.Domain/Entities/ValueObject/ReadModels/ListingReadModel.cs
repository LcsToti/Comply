namespace ProductService.Domain.Entities.ValueObject.ReadModels;

public record ListingReadModel(
    Guid Id,
    Guid SellerId,
    Guid ProductId,
    string Status,
    decimal BuyPrice,
    bool IsAuctionActive,
    bool IsProcessingPurchase,
    Guid? BuyerId,
    Guid? AuctionId,
    AuctionReadModel? Auction,
    DateTime ListedAt,
    DateTime UpdatedAt);