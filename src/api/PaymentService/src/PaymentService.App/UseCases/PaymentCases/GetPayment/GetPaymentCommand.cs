using MediatR;
using Payments.App.Common;
using Payments.App.Common.Results;

namespace Payments.App.UseCases.PaymentCases.GetPayment
{
    public record GetPaymentCommand(Guid PaymentId, Guid UserId, string Role) : IRequest<Result<PaymentResult>>;
}
