using MediatR;
using NotificationService.App.Results;

namespace NotificationService.App.UseCases.MarkNotificationAsRead
{
    public class MarkAsReadCommand : IRequest<Result>
    {
        public Guid NotificationId { get; set; }
        public Guid UserId { get; set; }
    }
}
