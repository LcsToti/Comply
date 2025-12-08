using MongoDB.Driver;
using SalesService.Domain.Aggregates.SaleAggregate.Entities;
using SalesService.Domain.Contracts;
using SalesService.Infra.Persistence.Document;
using SalesService.Infra.Persistence.Mappers;

namespace SalesService.Infra.Persistence.Repository;

public class SaleRepository : ISaleRepository<Sale>
{
    private readonly IMongoCollection<SaleDocument> _collection;

    public SaleRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<SaleDocument>("Sales");
    }

    public async Task AddAsync(Sale entity)
    {
        var document = SaleMapper.ToDocument(entity);
        await _collection.InsertOneAsync(document);
    }

    public async Task<Sale> UpdateAsync(Sale entity)
    {
        var document = SaleMapper.ToDocument(entity);
        var filter = Builders<SaleDocument>.Filter.Eq(d => d.Id, entity.Id);

        await _collection.ReplaceOneAsync(filter, document, new ReplaceOptions { IsUpsert = false });
        return entity;
    }

    public async Task<Sale> DeleteAsync(Sale entity)
    {
        var filter = Builders<SaleDocument>.Filter.Eq(d => d.Id, entity.Id);
        await _collection.DeleteOneAsync(filter);
        return entity;
    }

    public async Task<List<Sale>> GetAllAsync()
    {
        var documents = await _collection.Find(_ => true).ToListAsync();
        return documents.Select(d => d.ToDomain()).ToList();
    }

    public async Task<Sale?> GetByIdAsync(Guid id)
    {
        var filter = Builders<SaleDocument>.Filter.Eq(d => d.Id, id);
        var document = await _collection.Find(filter).FirstOrDefaultAsync();

        return document?.ToDomain();
    }

    public async Task<IEnumerable<Sale>?> GetAllByUserPagedAsync(Guid userId, int page, int pageSize)
    {
        var filter = Builders<SaleDocument>.Filter.Or(
            Builders<SaleDocument>.Filter.Eq(d => d.BuyerId, userId),
            Builders<SaleDocument>.Filter.Eq(d => d.SellerId, userId)
        );

        var documents = await _collection.Find(filter)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();

        return documents.Select(d => d.ToDomain());
    }

    public async Task<IEnumerable<Sale>?> GetAllDisputesByUserPagedAsync(Guid userId, int page, int pageSize)
    {
        var filter = Builders<SaleDocument>.Filter.And(
            Builders<SaleDocument>.Filter.Or(
                Builders<SaleDocument>.Filter.Eq(d => d.BuyerId, userId),
                Builders<SaleDocument>.Filter.Eq(d => d.SellerId, userId)
            ),
            Builders<SaleDocument>.Filter.Ne(d => d.Dispute, null)
        );

        var documents = await _collection.Find(filter)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();

        return documents.Select(d => d.ToDomain());
    }
}
