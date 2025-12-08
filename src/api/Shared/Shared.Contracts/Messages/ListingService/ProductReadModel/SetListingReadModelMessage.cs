using Shared.Contracts.Messages.ListingService.Common;

namespace Shared.Contracts.Messages.ListingService.ProductReadModel;

public record SetListingReadModelMessage(
    Guid ListingId,
    Guid SellerId,
    Guid ProductId,
    string Status,
    decimal BuyPrice,
    bool IsAuctionActive,
    AuctionReadModelMessage? Auction,
    bool IsProcessingPurchase,
    Guid? BuyerId,
    Guid? AuctionId,
    DateTime ListedAt,
    DateTime UpdatedAt);