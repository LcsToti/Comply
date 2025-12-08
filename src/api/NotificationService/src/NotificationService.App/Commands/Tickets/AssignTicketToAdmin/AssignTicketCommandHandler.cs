using NotificationService.Domain.Enums;
using DomainNotification = NotificationService.Domain.Entities.Notification;
using MediatR;
using NotificationService.App.Commands.Tickets.AssignTicketToAdmin;
using NotificationService.App.Results;
using NotificationService.Domain.Contracts;

namespace NotificationService.App.UseCases.AssignTicketToAdmin
{
    public class AssignTicketCommandHandler : IRequestHandler<AssignTicketCommand, Result>
    {
        private readonly ITicketRepository _ticketRepository;
        public AssignTicketCommandHandler(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public async Task<Result> Handle(AssignTicketCommand request, CancellationToken cancellationToken)
        {
            var ticket = await _ticketRepository.GetByIdAsync(request.TicketId);

            if (ticket == null)
            {
                return Result.Fail("Ticket not found.");
            }

            try
            {
                if (ticket.AssignedToAdminId == null && request.AdminId != Guid.Empty)
                {
                    ticket.AssignToAdmin(request.AdminId);
                }

                if (ticket.Status != request.NewStatus)
                {
                    ticket.UpdateStatus(request.NewStatus);
                }

                await _ticketRepository.UpdateAsync(ticket);

                if (request.NewStatus == TicketStatus.InProgress || request.NewStatus == TicketStatus.Closed)
                {
                    string message = $"Seu ticket de suporte #{ticket.Id} foi atualizado para: {request.NewStatus}.";

                    // await _externalNotifier.SendNotificationAsync(
                    //     DomainNotification.Create(ticket.UserId, NotificationType.TicketUpdated, new DomainSource("Ticket", ticket.Id), message
                    // ));
                    //TODO: Implement MASSTRANSIT and Create TYPE
                }

                return Result.Success();
            }
            catch (InvalidOperationException ex)
            {
                return Result.Fail(ex.Message);
            }
        }
    }
}
