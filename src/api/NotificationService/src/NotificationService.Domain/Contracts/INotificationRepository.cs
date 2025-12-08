using NotificationService.Domain.Entities;
using DomainNotification = NotificationService.Domain.Entities.Notification;

namespace NotificationService.Domain.Contracts
{
    public interface INotificationRepository
    {
        Task SaveAsync(Notification notification);
        Task UpdateAsync(Notification notification);
        Task<Notification> GetByIdAsync(Guid id);
        Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId, int page, int pageSize);
    }
}
