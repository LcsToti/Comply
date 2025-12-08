namespace Shared.Contracts.Messages.PaymentsService;

public record ApproveWithdrawalMessage(Guid PaymentId, Guid UserId);