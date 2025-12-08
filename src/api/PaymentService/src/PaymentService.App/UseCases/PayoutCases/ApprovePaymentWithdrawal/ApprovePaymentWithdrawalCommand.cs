using MediatR;
using Payments.App.Common;
using Payments.App.Common.Results;

namespace Payments.App.UseCases.PayoutCases.ApprovePaymentWithdrawal;

public record ApprovePaymentWithdrawalCommand(Guid PaymentId) : IRequest<Result<PaymentResult>>;