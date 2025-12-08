using Payments.Domain.Aggregates.PaymentAggregate.Entities;

namespace Payments.App.Common.Results.Mappers;

public static class ToPaymentMethodResultMapper
{
    public static PaymentMethodResult ToPaymentMethodResult(this PaymentMethod paymentMethod)
    {
        return new PaymentMethodResult(
            paymentMethod.Id,
            paymentMethod.Type,
            paymentMethod.Last4,
            paymentMethod.Brand
        );
    }
}