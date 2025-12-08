using MongoDB.Driver;
using NotificationService.Domain.Contracts;
using NotificationService.Domain.Entities;
using NotificationService.Infra.Persistence.DataModel;
using NotificationService.Infra.Persistence.Mappers;

namespace NotificationService.Infra.Persistence.Repositories;

public class WatchListRepository(IMongoDatabase database) : IWatchListRepository
{
    private readonly IMongoCollection<WatchListDataModel> _collection = database.GetCollection<WatchListDataModel>("WatchLists");

    public async Task<WatchList?> GetByUserIdAsync(Guid userId)
    {
        var filter = Builders<WatchListDataModel>.Filter.Eq(w => w.UserId, userId);
        var watchList = await _collection.Find(filter).FirstOrDefaultAsync();
        return watchList?.ToDomain();
    }

    public async Task SaveAsync(WatchList watchList)
    {
        var watchListDataModel = watchList.ToDataModel();
        var filter = Builders<WatchListDataModel>.Filter.Eq(w => w.UserId, watchList.UserId);
        var options = new ReplaceOptions { IsUpsert = true };

        await _collection.ReplaceOneAsync(filter, watchListDataModel, options);
    }

    public async Task<IEnumerable<Guid>> GetUsersWatchingProductAsync(Guid productId)
    {
        var filter = Builders<WatchListDataModel>.Filter.Exists($"ProductsWatching.{productId}");
        var watchLists = await _collection.Find(filter).ToListAsync();

        return watchLists.Select(w => w.UserId);
    }
}