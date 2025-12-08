namespace NotificationService.API.Requests;

public record CreateTicketRequest(Guid UserId, string Title, string Description);