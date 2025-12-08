using MediatR;
using Payments.App.Common;

namespace Payments.App.UseCases.UserCases.CreatePaymentMethods;

public record CreatePaymentMethodsCommand(Guid UserId) : IRequest<Result<string>>;