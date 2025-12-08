namespace Shared.Contracts.Messages.ListingService.Notifications.Bid;

public record BidPlacedNotificationMessage(
    Guid ProductId,
    Guid SellerId,
    Guid BidderId,
    decimal NewBidValue,
    DateTime PlacedAt);