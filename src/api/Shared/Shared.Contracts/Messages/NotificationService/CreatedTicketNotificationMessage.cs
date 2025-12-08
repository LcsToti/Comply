namespace Shared.Contracts.Messages.NotificationService;

public record CreatedTicketNotificationMessage(Guid TicketId, Guid AuthorId);