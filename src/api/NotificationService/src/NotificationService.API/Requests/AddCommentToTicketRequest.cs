namespace NotificationService.API.Requests;

public record AddCommentToTicketRequest(Guid TicketId, Guid AuthorId, string Content);