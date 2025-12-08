using ListingService.Domain.ListingAggregate;
using ListingService.Domain.Repositories;
using ListingService.Infra.Persistence.DataModels;
using ListingService.Infra.Persistence.Mappers;
using MongoDB.Driver;

namespace ListingService.Infra.Persistence.Repositories;

internal class ListingRepository(IMongoDatabase database) : IListingRepository
{
    private readonly IMongoCollection<ListingDataModel> _collection = database.GetCollection<ListingDataModel>("Listings");

    public async Task AddAsync(Listing entity)
    {
        var dm = entity.ToDataModel();
        await _collection.InsertOneAsync(dm);
    }

    public async Task<Listing?> GetByIdAsync(Guid id)
    {
        var dm = await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        return dm?.ToDomain();
    }

    public async Task UpdateAsync(Listing entity)
    {
        var dm = entity.ToDataModel();
        await _collection.ReplaceOneAsync(x => x.Id == dm.Id, dm);
    }

    public async Task DeleteAsync(Listing entity)
    {
        await _collection.DeleteOneAsync(x => x.Id == entity.Id);
    }

    public async Task<Listing?> GetByProductIdAsync(Guid productId)
    {
        var dm = await _collection.Find(x => x.ProductId == productId).FirstOrDefaultAsync();
        return dm?.ToDomain();
    }

    public async Task<bool> AtomicPrepareBuyNowAsync(Guid listingId)
    {
        var filter = Builders<ListingDataModel>.Filter.And(
            Builders<ListingDataModel>.Filter.Eq(x => x.Id, listingId),
            Builders<ListingDataModel>.Filter.Eq(x => x.IsProcessingPurchase, false));

        var update = Builders<ListingDataModel>.Update
            .Set(x => x.IsProcessingPurchase, true);

        var result = await _collection.UpdateOneAsync(filter, update);

        return result.ModifiedCount > 0;
    }

    public async Task ReleaseBuyNowLockAsync(Guid listingId)
    {
        var filter = Builders<ListingDataModel>.Filter.Eq(x => x.Id, listingId);

        var update = Builders<ListingDataModel>.Update
            .Set(x => x.IsProcessingPurchase, false);

        await _collection.UpdateOneAsync(filter, update);
    }
}