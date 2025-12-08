using MediatR;
using NotificationService.App.Results;
using NotificationService.Domain.Contracts;
using NotificationService.Domain.VOs;

namespace NotificationService.App.Commands.Tickets.AddCommentToTicket
{
    public class AddCommentToTicketCommandHandler : IRequestHandler<AddCommentToTicketCommand, Result>
    {
        private readonly ITicketRepository _ticketRepository;

        public AddCommentToTicketCommandHandler(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public async Task<Result> Handle(AddCommentToTicketCommand request, CancellationToken cancellationToken)
        {
            var ticket = await _ticketRepository.GetByIdAsync(request.TicketId);

            if (ticket == null)
            {
                return Result.Fail("Ticket de suporte não encontrado.");
            }

            Comment comment;
            try
            {
                comment = Comment.Create(request.AuthorId, request.Content);
            }
            catch (ArgumentException ex)
            {
                return Result.Fail(ex.Message);
            }

            ticket.AddComment(comment);

            await _ticketRepository.UpdateAsync(ticket);

            if (ticket.AssignedToAdminId.HasValue && ticket.AssignedToAdminId.Value != request.AuthorId)
            {
            }


            return Result.Success();
        }
    }
}