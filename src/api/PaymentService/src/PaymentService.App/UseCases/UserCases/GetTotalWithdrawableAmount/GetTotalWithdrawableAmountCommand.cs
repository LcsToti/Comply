using MediatR;
using Payments.App.Common;

namespace Payments.App.UseCases.UserCases.GetTotalWithdrawableAmount;

public record GetTotalWithdrawableAmountCommand(Guid UserId) : IRequest<Result<decimal>>;