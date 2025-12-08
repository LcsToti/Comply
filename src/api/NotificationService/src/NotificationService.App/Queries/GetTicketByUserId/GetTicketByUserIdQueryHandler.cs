using MediatR;
using NotificationService.App.Results;
using NotificationService.App.Results.Mappers;
using NotificationService.Domain.Contracts;

namespace NotificationService.App.Queries.GetTicketByUserId;

public class GetTicketByUserIdQueryHandler(ITicketRepository ticketRepository) : IRequestHandler<GetTicketByUserIdQuery, Result<List<TicketResult>>>
{
    private readonly ITicketRepository _ticketRepository = ticketRepository;
    
    public async Task<Result<List<TicketResult>>> Handle(GetTicketByUserIdQuery request, CancellationToken cancellationToken)
    {
        var ticketsList = await _ticketRepository.GetByUserIdAsync(request.UserId);
        var ticketsListResult = ticketsList.Select(t => t.ToTicketResult()).ToList();

        return Result<List<TicketResult>>.Success(ticketsListResult);
    }
}