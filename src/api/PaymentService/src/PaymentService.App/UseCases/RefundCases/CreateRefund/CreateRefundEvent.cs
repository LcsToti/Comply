using MediatR;
using Payments.App.Common.Contracts;

namespace Payments.App.UseCases.RefundCases.CreateRefund
{
    public record CreateRefundEvent(decimal Amount, Guid PaymentId, string Reason, Guid UserId, IPaymentPublisher Publisher) : IRequest;
}
