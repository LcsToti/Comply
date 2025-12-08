namespace Shared.Contracts.Messages.PaymentsService.Purchase;

public record PurchasePaymentRefundFailedMessage(Guid ListingId, Guid BuyerId);