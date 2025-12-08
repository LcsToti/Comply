using MediatR;
using Payments.App.Common;
using Payments.App.Common.Results;

namespace Payments.App.UseCases.PaymentCases.GetUserPayments
{
    public record GetPayerPaymentsCommand(Guid UserId) : IRequest<Result<PaymentResult[]>>;
}
