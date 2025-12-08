using NotificationService.Domain.VOs;

namespace NotificationService.App.Results;

public record TicketResult(
    Guid Id,
    Guid UserId,
    string Title,
    string Description,
    DateTime CreatedAt,
    string Status,
    DateTime UpdatedAt,
    Guid? AssignedAdminId,
    IReadOnlyList<Comment> Comments);