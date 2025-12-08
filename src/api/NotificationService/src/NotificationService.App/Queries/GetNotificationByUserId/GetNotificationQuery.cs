using MediatR;
using NotificationService.App.Results;
using NotificationService.Domain.Enums;

namespace NotificationService.App.Queries.GetNotificationByUserId
{
    public class NotificationResponse
    {
        public Guid Id { get; set; }
        public NotificationType Type { get; set; }
        public string Message { get; set; }
        public bool Read { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class GetNotificationsQuery : IRequest<Result<IEnumerable<NotificationResponse>>>
    {
        public Guid UserId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
    
    //TODO: Transform to Records and Command
}
