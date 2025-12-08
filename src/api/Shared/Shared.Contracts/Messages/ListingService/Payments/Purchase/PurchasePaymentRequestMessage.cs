namespace Shared.Contracts.Messages.ListingService.Payments.Purchase;

public record PurchasePaymentRequestMessage(
    Guid ListingId,
    Guid BuyerId,
    DateTime ExpiresAt,
    decimal Value,
    string PaymentMethod,
    Guid SellerId);