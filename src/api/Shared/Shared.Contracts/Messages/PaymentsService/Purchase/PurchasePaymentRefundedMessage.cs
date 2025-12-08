namespace Shared.Contracts.Messages.PaymentsService.Purchase;

public record PurchasePaymentRefundedMessage(Guid ListingId, Guid BuyerId);