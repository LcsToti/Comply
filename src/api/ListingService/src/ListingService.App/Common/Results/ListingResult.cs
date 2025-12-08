namespace ListingService.App.Common.Results;

public record ListingResult(
    Guid Id,
    Guid SellerId,
    Guid ProductId,
    string Status,
    decimal BuyPrice,
    bool IsAuctionActive,
    Guid? BuyerId,
    Guid? AuctionId,
    DateTime ListedAt,
    DateTime UpdatedAt);