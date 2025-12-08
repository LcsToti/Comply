using NotificationService.Domain.Enums;

namespace NotificationService.API.Requests;

public record AssignTicketRequest(TicketStatus NewStatus);