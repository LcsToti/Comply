
namespace Payments.Domain.Aggregates.PaymentAggregate.Enums
{
    public enum PaymentStatus
    {
        Pending,
        Succeeded,
        Failed,
        Canceled,
        Refunded,
        PartiallyRefunded
    }
}
