using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using ProductService.Application.Constants;
using ProductService.Domain.Contracts;
using ProductService.Domain.Entities;
using ProductService.Domain.Entities.ValueObjects;

namespace ProductService.Infrastructure.Persistence.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly IMongoCollection<Product> _products;

    public ProductRepository(IMongoDatabase database)
    {
        _products = database.GetCollection<Product>("Products");
    }

    public async Task AddAsync(Product product)
    {
        await _products.InsertOneAsync(product);
    }

    public async Task<Product?> GetByIdAsync(Guid productId)
    {
        return await _products.Find(p => p.ProductId == productId).SingleOrDefaultAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.ProductId, product.ProductId);

        var updateDefinitions = new List<UpdateDefinition<Product>>
        {
            Builders<Product>.Update.Set(p => p.Title, product.Title),
            Builders<Product>.Update.Set(p => p.Description, product.Description),
            Builders<Product>.Update.Set(p => p.Images, product.Images),
            Builders<Product>.Update.Set(p => p.Locale, product.Locale),
            Builders<Product>.Update.Set(p => p.Characteristics, product.Characteristics),
            Builders<Product>.Update.Set(p => p.Condition, product.Condition),
            Builders<Product>.Update.Set(p => p.Category, product.Category),
            Builders<Product>.Update.Set(p => p.Featured, product.Featured),
            Builders<Product>.Update.Set(p => p.DeliveryPreference, product.DeliveryPreference),
            Builders<Product>.Update.Set(p => p.UpdatedAt, product.UpdatedAt),
            Builders<Product>.Update.Set(p => p.Qna.Questions, product.Qna.Questions),
            Builders<Product>.Update.Set(p => p.Qna.TotalQuestions, product.Qna.TotalQuestions),
            Builders<Product>.Update.Set(p => p.Listing, product.Listing)
        };

        var combinedUpdate = Builders<Product>.Update.Combine(updateDefinitions);

        await _products.UpdateOneAsync(filter, combinedUpdate);
    }

    public async Task UpdateQnaAsync(Guid productId, Guid questionId, Answer? answer, Question? question)
    {
        var filter = Builders<Product>.Filter.And(
            Builders<Product>.Filter.Eq(p => p.ProductId, productId),
            Builders<Product>.Filter.ElemMatch(p => p.Qna.Questions, q => q.QuestionId == questionId)
        );

        var update = new List<UpdateDefinition<Product>>();

        if (answer != null)
        {
            update.Add(Builders<Product>.Update.Set("Qna.Questions.$.Answer", answer));
        }

        if (question != null)
        {
            update.Add(Builders<Product>.Update.Set("Qna.Questions.$.QuestionText", question.QuestionText));
            update.Add(Builders<Product>.Update.Set("Qna.Questions.$.AskedAt", question.AskedAt));
        }

        var combinedUpdate = Builders<Product>.Update.Combine(update);

        await _products.UpdateOneAsync(filter, combinedUpdate);
    }

    public async Task UpdateWatchListCountAsync(Guid productId, int watchListCount)
    {
        var update = Builders<Product>.Update.Set(p => p.WatchListCount, watchListCount);
        await _products.UpdateOneAsync(p => p.ProductId == productId, update);
    }

    public async Task<long> GetProductsCountAsync()
    {
        var count = await _products.CountDocumentsAsync(_ => true);
        return count;
    }

    public async Task<long> GetActiveAuctionsCountAsync()
    {
        var filter = Builders<Product>.Filter.Eq(ProductFields.ListingStatus, "Active");
        
        var count = await _products.CountDocumentsAsync(filter);
        return count;
    }

    public async Task DeleteAsync(Guid productId)
    {
        await _products.DeleteOneAsync(p => p.ProductId == productId);
    }
}