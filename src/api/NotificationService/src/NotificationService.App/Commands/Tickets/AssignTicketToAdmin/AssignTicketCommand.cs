using MediatR;
using NotificationService.App.Results;
using NotificationService.Domain.Enums;

namespace NotificationService.App.Commands.Tickets.AssignTicketToAdmin
{
    public record AssignTicketCommand(Guid TicketId, Guid AdminId, TicketStatus NewStatus) : IRequest<Result>;
}
