namespace Shared.Contracts.Messages.UserService;

public record CreatedUserMessage(
    string Email, 
    string Name, 
    Guid UserId);
