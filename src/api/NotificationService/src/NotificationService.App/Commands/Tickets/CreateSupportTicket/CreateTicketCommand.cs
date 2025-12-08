using MediatR;
using NotificationService.App.Results;

namespace NotificationService.App.Commands.Tickets.CreateSupportTicket
{
    public record CreateTicketCommand(Guid UserId, string Title, string Description) : IRequest<Result<Guid>>;
}
