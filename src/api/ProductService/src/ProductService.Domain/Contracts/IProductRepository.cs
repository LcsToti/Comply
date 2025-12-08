using ProductService.Domain.Entities;
using ProductService.Domain.Entities.ValueObjects;

namespace ProductService.Domain.Contracts
{
    public interface IProductRepository
    {
        Task AddAsync(Product product);
        Task<Product?> GetByIdAsync(Guid productId);
        Task UpdateAsync(Product product);
        Task UpdateQnaAsync(Guid productId, Guid questionId, Answer? answer, Question? question);
        Task DeleteAsync(Guid productId);
        Task UpdateWatchListCountAsync(Guid productId, int watchListCount);
        Task<long> GetProductsCountAsync();
        Task<long> GetActiveAuctionsCountAsync();
    }
}
