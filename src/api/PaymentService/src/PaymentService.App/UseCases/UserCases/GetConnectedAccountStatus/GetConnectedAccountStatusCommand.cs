using MediatR;
using Payments.App.Common;
using Payments.Domain.Aggregates.PaymentAccountAggregate;

namespace Payments.App.UseCases.UserCases.GetConnectedAccountStatus;

public record GetConnectedAccountStatusCommand(Guid UserId) : IRequest<Result<PaymentAccountStatus>>;