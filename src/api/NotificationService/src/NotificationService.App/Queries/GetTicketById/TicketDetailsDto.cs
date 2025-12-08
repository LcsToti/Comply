using NotificationService.Domain.Enums;

namespace NotificationService.App.UseCases.GetTicketById
{
    public class TicketDetailsDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public TicketStatus Status { get; set; }
        public Guid? AssignedToAdminId { get; set; }
    }
}