using MediatR;
using Payments.App.Common.Contracts;

namespace Payments.App.UseCases.PaymentCases.CreatePayment
{
    public record CreatePaymentEvent(Guid SourceId, Guid BuyerId, Guid SellerId, DateTime ExpiresAt, decimal Value, string PaymentMethod, IPaymentPublisher Publisher) : IRequest;
}
