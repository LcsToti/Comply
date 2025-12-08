using MediatR;
using NotificationService.App.Results;

namespace NotificationService.App.UseCases.GetTicketById
{
    public class GetTicketByIdQuery : IRequest<Result<TicketDetailsDto>>
    {
        public Guid Id { get; }

        public GetTicketByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}