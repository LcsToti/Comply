namespace Payments.Domain.Aggregates.PaymentAggregate.Enums;

public enum WithdrawalStatus
{
    WaitingApproval,
    ApprovedToWithdraw,
    Withdrawing,
    AlreadyWithdrawn,
    Failed
}