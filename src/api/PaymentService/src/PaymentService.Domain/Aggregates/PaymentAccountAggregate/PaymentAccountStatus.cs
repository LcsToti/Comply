namespace Payments.Domain.Aggregates.PaymentAccountAggregate;

public enum PaymentAccountStatus
{
    None,
    Incomplete,
    PendingReview,
    Issues,
    Active  
}