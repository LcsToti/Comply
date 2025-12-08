using MediatR;
using Payments.App.Common;

namespace Payments.App.UseCases.UserCases.GetUserDashboardLink;

public record GetUserDashboardLinkCommand(Guid UserId) : IRequest<Result<string>>;