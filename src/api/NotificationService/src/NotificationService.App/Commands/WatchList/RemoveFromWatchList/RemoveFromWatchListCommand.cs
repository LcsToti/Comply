using MediatR;

namespace NotificationService.App.Commands.WatchList.RemoveFromWatchList;

public record RemoveFromWatchListCommand(Guid UserId, Guid ProductId) : IRequest;