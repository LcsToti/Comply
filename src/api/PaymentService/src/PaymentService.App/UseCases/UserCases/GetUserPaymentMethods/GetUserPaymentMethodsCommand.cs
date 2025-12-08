using MediatR;
using Payments.App.Common;
using Payments.App.Common.Results;

namespace Payments.App.UseCases.UserCases.GetUserPaymentMethods;

public record GetUserPaymentMethodsCommand(Guid UserId) : IRequest<Result<IReadOnlyCollection<PaymentMethodResult>>>;