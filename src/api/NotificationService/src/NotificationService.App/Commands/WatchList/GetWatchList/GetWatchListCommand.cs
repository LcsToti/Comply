using MediatR;

namespace NotificationService.App.Commands.WatchList.GetWatchList;

public record GetWatchListCommand(Guid UserId) : IRequest<List<Guid>>;