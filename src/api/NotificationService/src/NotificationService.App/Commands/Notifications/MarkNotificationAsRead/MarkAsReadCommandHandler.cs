using MediatR;
using NotificationService.App.Results;
using NotificationService.Domain.Contracts;

namespace NotificationService.App.UseCases.MarkNotificationAsRead
{
    public class MarkAsReadCommandHandler : IRequestHandler<MarkAsReadCommand, Result>
    {
        private readonly INotificationRepository _notificationRepository;

        public MarkAsReadCommandHandler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<Result> Handle(MarkAsReadCommand request, CancellationToken cancellationToken)
        {
            var notification = await _notificationRepository.GetByIdAsync(request.NotificationId);

            if (notification == null)
            {
                return Result.Fail("Notification not found.");
            }

            if (notification.UserId != request.UserId)
            {
                return Result.Fail("User is not authorized to read this notification.");
            }

            if (!notification.Read)
            {
                notification.MarkAsRead();

                await _notificationRepository.UpdateAsync(notification);
            }

            return Result.Success();
        }
    }
}
