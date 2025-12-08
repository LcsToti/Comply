namespace Shared.Contracts.Messages.ListingService.Payments.Purchase;

public record PurchaseRefundRequestMessage(
    decimal AmountToRefund,
    Guid PaymentId, 
    string Reason, 
    Guid UserId);