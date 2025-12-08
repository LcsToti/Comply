using MediatR;
using NotificationService.App.Results;
using NotificationService.Domain.Contracts;

namespace NotificationService.App.Queries.GetNotificationByUserId
{
    public class GetNotificationQueryHandler : IRequestHandler<GetNotificationsQuery, Result<IEnumerable<NotificationResponse>>>
    {
        private readonly INotificationRepository _notificationRepository;

        public GetNotificationQueryHandler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<Result<IEnumerable<NotificationResponse>>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
        {
            var notifications = await _notificationRepository.GetByUserIdAsync(
                request.UserId,
                request.Page,
                request.PageSize
            );

            var enumerable = notifications.ToList();
            if (!enumerable.Any())
            {
                return Result<IEnumerable<NotificationResponse>>.Success([]);
            }

            var response = enumerable.Select(n => new NotificationResponse
            {
                Id = n.Id,
                Type = n.Type,
                Message = n.Message,
                Read = n.Read,
                CreatedAt = n.CreatedAt
            });

            return Result<IEnumerable<NotificationResponse>>.Success(response);
        }
    }
}
