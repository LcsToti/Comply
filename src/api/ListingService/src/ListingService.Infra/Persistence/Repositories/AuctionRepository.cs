using ListingService.Domain.AuctionAggregate.Entities;
using ListingService.Domain.Repositories;
using ListingService.Infra.Persistence.DataModels;
using ListingService.Infra.Persistence.Mappers;
using MongoDB.Driver;

namespace ListingService.Infra.Persistence.Repositories;

internal class AuctionRepository(IMongoDatabase database) : IAuctionRepository
{
    private readonly IMongoCollection<AuctionDataModel> _collection = database.GetCollection<AuctionDataModel>("Auctions");

    public async Task AddAsync(Auction entity)
    {
        var dm = entity.ToDataModel();
        await _collection.InsertOneAsync(dm);
    }

    public async Task<Auction?> GetByIdAsync(Guid id)
    {
        var dm = await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        return dm?.ToDomain();
    }

    public async Task UpdateAsync(Auction entity)
    {
        var dm = entity.ToDataModel();
        await _collection.ReplaceOneAsync(x => x.Id == dm.Id, dm);
    }

    public async Task DeleteAsync(Auction entity)
    {
        await _collection.DeleteOneAsync(x => x.Id == entity.Id);
    }

    public async Task<List<Auction>> GetAuctionsByListingIdAsync(Guid listingId)
    {
        var dms = await _collection.Find(x => x.ListingId == listingId).ToListAsync();

        return dms is null || dms.Count == 0 ? [] : [.. dms.Select(dm => dm.ToDomain())];
    }

    public async Task<bool> AtomicPrepareNewBidAsync(Guid auctionId)
    {
        var filter = Builders<AuctionDataModel>.Filter.And(
            Builders<AuctionDataModel>.Filter.Eq(x => x.Id, auctionId),
            Builders<AuctionDataModel>.Filter.Eq(x => x.IsProcessingBid, false));

        var update = Builders<AuctionDataModel>.Update
            .Set(x => x.IsProcessingBid, true);

        var result = await _collection.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }

    public async Task ReleaseNewBidLockAsync(Guid auctionId)
    {
        var filter = Builders<AuctionDataModel>.Filter.Eq(x => x.Id, auctionId);
     
        var update = Builders<AuctionDataModel>.Update
            .Set(x => x.IsProcessingBid, false);

        await _collection.UpdateOneAsync(filter, update);
    }
}