using Payments.Domain.Aggregates.PaymentAggregate.Entities;

namespace Payments.App.Common.Results.Mappers;

public static class ToPaymentResultMapper
{
    public static PaymentResult ToPaymentResult(this Payment payment)
    {
        return new PaymentResult(
            payment.Id,
            payment.WithdrawalStatus.ToString(),
            payment.Status.ToString(),
            payment.PayerId,
            payment.Amount,
            payment.Timestamps
        );
    }
}