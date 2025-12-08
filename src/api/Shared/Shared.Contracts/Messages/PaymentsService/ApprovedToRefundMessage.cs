namespace Shared.Contracts.Messages.PaymentsService;

public record ApprovedToRefundMessage(
    Guid PaymentId, 
    decimal AmountToRefund, 
    string Reason, 
    Guid UserId);