using MediatR;
using NotificationService.App.Results;
using NotificationService.Domain.Contracts;
using NotificationService.Domain.Entities;

namespace NotificationService.App.UseCases.GetTicketById
{
    public class GetTicketByIdQueryHandler : IRequestHandler<GetTicketByIdQuery, Result<TicketDetailsDto>>
    {
        private readonly ITicketRepository _ticketRepository;

        public GetTicketByIdQueryHandler(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public async Task<Result<TicketDetailsDto>> Handle(GetTicketByIdQuery request, CancellationToken cancellationToken)
        {
            var ticket = await _ticketRepository.GetByIdAsync(request.Id);

            if (ticket == null)
            {
                return Result<TicketDetailsDto>.Fail("Ticket de suporte não encontrado.");
            }

            var ticketDto = MapToDto(ticket);

            return Result<TicketDetailsDto>.Success(ticketDto);
        }
        private static TicketDetailsDto MapToDto(SupportTicket ticket)
        {
            return new TicketDetailsDto
            {
                Id = ticket.Id,
                UserId = ticket.UserId,
                Title = ticket.Title,
                Description = ticket.Description,
                CreatedAt = ticket.CreatedAt,
                Status = ticket.Status,
                AssignedToAdminId = ticket.AssignedToAdminId
            };
        }
    }
}