using MongoDB.Driver;
using NotificationService.Domain.Contracts;
using NotificationService.Domain.Entities;
using NotificationService.Infra.Persistence.DataModel;
using NotificationService.Infra.Persistence.Mappers;

namespace NotificationService.Infra.Persistence.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly IMongoCollection<NotificationDataModel> _collection;

        public NotificationRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<NotificationDataModel>("Notifications");
        }

        public async Task<Notification> GetByIdAsync(Guid id)
        {
            var notification = await _collection
                .Find(n => n.Id == id)
                .FirstOrDefaultAsync();

            return notification.ToDomain();
        }

        public async Task SaveAsync(Notification notification)
        {
            var notificationDataModel = notification.ToDataModel();
            
            var options = new ReplaceOptions { IsUpsert = true };

            await _collection.ReplaceOneAsync(
                filter: n => n.Id == notification.Id,
                replacement: notificationDataModel,
                options
            );
        }

        public Task UpdateAsync(Notification notification) => SaveAsync(notification);

        public async Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId, int page, int pageSize)
        {
            var skip = (page - 1) * pageSize;

            var notifications = await _collection
                .Find(n => n.UserId == userId)
                .SortByDescending(n => n.CreatedAt)
                .Skip(skip)
                .Limit(pageSize)
                .ToListAsync();
            
            return notifications.Select(n => n.ToDomain());
        }
    }
}