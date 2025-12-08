using MediatR;
using NotificationService.App.Results;
using NotificationService.Domain.Enums;

namespace NotificationService.App.Events.ProcessNotificationEvent
{
    public record ProcessNotificationEvent(NotificationType NotificationType, Guid UserId, string Message, Guid? SourceId) : IRequest;
}
