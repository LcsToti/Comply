using MediatR;
using NotificationService.App.Results;

namespace NotificationService.App.Queries.GetAllTickets;

public record GetAllTicketsQuery() : IRequest<Result<List<TicketResult>>>;