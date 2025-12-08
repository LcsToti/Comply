using NotificationService.Domain.Entities;

namespace NotificationService.App.Results.Mappers;

public static class TicketResultMapper
{
    public static TicketResult ToTicketResult(this SupportTicket ticket)
    {
        return new TicketResult(
            Id: ticket.Id,
            UserId: ticket.UserId,
            Title: ticket.Title,
            Description: ticket.Description,
            CreatedAt: ticket.CreatedAt,
            UpdatedAt: ticket.LastUpdateAt,
            Status: ticket.Status.ToString(),
            AssignedAdminId: ticket.AssignedToAdminId,
            Comments: ticket.Comments);
    }
}