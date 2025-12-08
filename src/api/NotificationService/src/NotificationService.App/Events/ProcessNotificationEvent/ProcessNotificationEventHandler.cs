using MediatR;
using Microsoft.Extensions.Logging;
using NotificationService.Domain.Contracts;
using DomainNotification = NotificationService.Domain.Entities.Notification;

namespace NotificationService.App.Events.ProcessNotificationEvent
{
    public class ProcessNotificationEventHandler : IRequestHandler<ProcessNotificationEvent>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly ILogger<ProcessNotificationEventHandler> _logger;
        public ProcessNotificationEventHandler(INotificationRepository notificationRepository, ILogger<ProcessNotificationEventHandler> logger)
        {
            _notificationRepository = notificationRepository;
            _logger = logger;
        }

        public async Task Handle(ProcessNotificationEvent request, CancellationToken cancellationToken)
        {
            //Domain
            var notification = DomainNotification.Create(request.UserId, request.NotificationType, request.Message);
            
            // Persist
            await _notificationRepository.SaveAsync(notification);
            _logger.LogInformation("Notification {NotificationId} saved.", notification.Id);
        }
    }
}