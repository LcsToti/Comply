namespace NotificationService.API.Requests;

public record AddToWatchListRequest(Guid UserId, Guid ProductId, Guid ListingId);