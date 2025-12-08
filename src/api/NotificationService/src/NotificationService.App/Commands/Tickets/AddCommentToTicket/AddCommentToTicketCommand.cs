using MediatR;
using NotificationService.App.Results;

namespace NotificationService.App.Commands.Tickets.AddCommentToTicket
{
    public record AddCommentToTicketCommand(Guid TicketId, Guid AuthorId, string Content) : IRequest<Result>;
}