using MediatR;
using Payments.App.Common;

namespace Payments.App.UseCases.UserCases.GetUserOnboardingLink;

public record GetUserOnboardingLinkCommand(Guid UserId) : IRequest<Result<string>>;