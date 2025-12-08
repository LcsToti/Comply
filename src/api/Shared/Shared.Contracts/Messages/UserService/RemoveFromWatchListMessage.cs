namespace Shared.Contracts.Messages.UserService;

public record RemoveFromWatchListMessage(
    Guid UserId, 
    Guid ProductId);