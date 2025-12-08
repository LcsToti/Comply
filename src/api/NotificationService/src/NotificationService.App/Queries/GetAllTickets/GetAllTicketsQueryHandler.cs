using MediatR;
using NotificationService.App.Results;
using NotificationService.App.Results.Mappers;
using NotificationService.Domain.Contracts;

namespace NotificationService.App.Queries.GetAllTickets;

public class GetAllTicketsQueryHandler(ITicketRepository ticketRepository) : IRequestHandler<GetAllTicketsQuery, Result<List<TicketResult>>>
{
    private readonly ITicketRepository _ticketRepository = ticketRepository;

    public async Task<Result<List<TicketResult>>> Handle(GetAllTicketsQuery request, CancellationToken cancellationToken)
    {
        var ticketsList = await _ticketRepository.GetAllAsync();
        var ticketsListResult = ticketsList.Select(t => t.ToTicketResult()).ToList();

        return Result<List<TicketResult>>.Success(ticketsListResult);
    }
}