using Payments.Domain.Aggregate.VOs;
using Payments.Domain.Aggregates.PaymentAggregate.Entities;
using Payments.Domain.Aggregates.PaymentAggregate.Enums;
using Payments.Domain.Aggregates.PaymentAggregate.Factories;
using Payments.Domain.Aggregates.PaymentAggregate.VOs;
using Stripe;
using DomainRefund = Payments.Domain.Aggregates.PaymentAggregate.Entities.Refund;
using StripeRefund = Stripe.Refund;

namespace Payments.Infra.PaymentGateway.Mappers
{
    public class StripeToDomainMapper
{
    public static Payment StripeToDomain(PaymentIntent stripeDto)
    {
        var gateway = Gateway.Create("STRIPE", stripeDto.Id, stripeDto.LatestChargeId);
        var amount = Amount.Create(stripeDto.Amount / 100m, stripeDto.Currency);
        
        var payment = PaymentFactory.CreatePayment(gateway, amount);
        
        var domainStatus = MapStatusFromStripe(stripeDto.Status);
        
        switch (domainStatus)
        {
            case PaymentStatus.Succeeded:
                payment.Confirm(stripeDto.PaymentMethodId);
                break;
            case PaymentStatus.Failed:
                payment.Fail();
                break;
            case PaymentStatus.Pending:
            case PaymentStatus.Canceled:
            case PaymentStatus.Refunded:
            case PaymentStatus.PartiallyRefunded:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return payment;
    }
    public static DomainRefund StripeToDomain(StripeRefund stripeDto)
    {
        if (stripeDto == null)
        {
            return null;
        }
        
        return RefundFactory.Create(
            stripeDto.Id,
            stripeDto.Amount,
            stripeDto.Reason,
            stripeDto.Status,
            stripeDto.Created
        );
    }
    private static PaymentStatus MapStatusFromStripe(string stripeStatus)
    {
        return stripeStatus switch
        {
            "succeeded" => PaymentStatus.Succeeded,
            "processing" => PaymentStatus.Pending,
            "requires_payment_method" => PaymentStatus.Pending,
            "requires_confirmation" => PaymentStatus.Pending,
            "requires_action" => PaymentStatus.Pending,
            "requires_capture" => PaymentStatus.Pending,
            "canceled" => PaymentStatus.Canceled,
            _ => PaymentStatus.Failed,
        };
    }
}
}

