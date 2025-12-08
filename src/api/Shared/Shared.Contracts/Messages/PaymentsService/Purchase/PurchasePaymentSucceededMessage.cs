namespace Shared.Contracts.Messages.PaymentsService.Purchase;

public record PurchasePaymentSucceededMessage(
    Guid ListingId,
    Guid BuyerId,
    DateTime ExpiresAt,
    Guid PaymentId,
    decimal Price);