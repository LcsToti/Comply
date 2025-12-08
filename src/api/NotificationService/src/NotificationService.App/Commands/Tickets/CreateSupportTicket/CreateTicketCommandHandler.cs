using MassTransit;
using MediatR;
using NotificationService.App.Results;
using NotificationService.Domain.Contracts;
using NotificationService.Domain.Entities;
using Shared.Contracts.Messages.NotificationService;

namespace NotificationService.App.Commands.Tickets.CreateSupportTicket
{
    public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, Result<Guid>>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IPublishEndpoint _publishEndpoint;

        public CreateTicketCommandHandler(ITicketRepository ticketRepository, IPublishEndpoint publishEndpoint)
        {
            _ticketRepository = ticketRepository;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<Result<Guid>> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var ticket = SupportTicket.Create(
                    request.UserId,
                    request.Title,
                    request.Description
                );

                await _ticketRepository.SaveAsync(ticket);
                await _publishEndpoint.Publish(new CreatedTicketNotificationMessage(ticket.Id, ticket.UserId), cancellationToken);

                return Result<Guid>.Success(ticket.Id);
            }
            catch (ArgumentException ex)
            {
                return Result<Guid>.Fail(ex.Message);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Fail("Falha ao criar ticket de suporte: " + ex.Message);
            }
        }
    }
}
