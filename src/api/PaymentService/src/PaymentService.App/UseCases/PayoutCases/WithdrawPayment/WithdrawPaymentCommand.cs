using MediatR;
using Payments.App.Common;
using Payments.App.Common.Results;

namespace Payments.App.UseCases.PayoutCases.WithdrawPayment;

public record WithdrawPaymentCommand(Guid PaymentId, Guid UserId) : IRequest<Result<PaymentResult>>;