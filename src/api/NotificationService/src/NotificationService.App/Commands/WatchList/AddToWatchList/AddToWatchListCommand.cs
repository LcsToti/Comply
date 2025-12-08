using MediatR;

namespace NotificationService.App.Commands.WatchList.AddToWatchList;

public record AddToWatchListCommand(Guid UserId, Guid ProductId, Guid ListingId) : IRequest;