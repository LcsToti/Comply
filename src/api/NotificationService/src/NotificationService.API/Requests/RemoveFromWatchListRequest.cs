namespace NotificationService.API.Requests;

public record RemoveFromWatchListRequest(Guid UserId, Guid ProductId);