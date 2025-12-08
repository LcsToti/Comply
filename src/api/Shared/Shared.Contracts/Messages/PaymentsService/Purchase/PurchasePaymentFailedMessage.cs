namespace Shared.Contracts.Messages.PaymentsService.Purchase;

public record PurchasePaymentFailedMessage(
    Guid ListingId,
    Guid BuyerId,
    DateTime ExpiresAt);
