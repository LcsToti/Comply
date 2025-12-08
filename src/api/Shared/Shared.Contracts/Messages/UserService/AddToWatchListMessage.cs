namespace Shared.Contracts.Messages.UserService;

public record AddToWatchListMessage(
    Guid UserId, 
    Guid ProductId,
    Guid ListingId);