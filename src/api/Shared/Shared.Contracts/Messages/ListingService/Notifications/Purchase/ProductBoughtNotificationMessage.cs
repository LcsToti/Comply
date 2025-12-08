namespace Shared.Contracts.Messages.ListingService.Notifications.Purchase;

public record ProductBoughtNotificationMessage(
    Guid ProductId,
    Guid SellerId,
    Guid BuyerId,
    decimal Price,
    DateTime BoughtAt);
