using MediatR;
using Payments.App.Common;
using Payments.App.Common.Results;

namespace Payments.App.UseCases.PayoutCases.WithdrawAllAvailablePayments;

public record WithdrawAllAvailablePaymentsCommand(Guid UserId) : IRequest<Result<PaymentResult[]>>;