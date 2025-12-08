using MediatR;
using NotificationService.App.Results;

namespace NotificationService.App.Queries.GetTicketByUserId;

public record GetTicketByUserIdQuery(Guid UserId) : IRequest<Result<List<TicketResult>>>;