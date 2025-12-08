using NotificationService.Domain.Entities;

namespace NotificationService.Domain.Contracts;

public interface IWatchListRepository
{
    Task<WatchList?> GetByUserIdAsync(Guid userId);
    Task SaveAsync(WatchList watchList);
    Task<IEnumerable<Guid>> GetUsersWatchingProductAsync(Guid productId);
}