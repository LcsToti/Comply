using SalesService.Domain.Common;

namespace SalesService.Domain.Contracts;

public interface ISaleRepository<TSale> where TSale : IAggregateRoot
{
    Task AddAsync(TSale entity);
    Task<TSale> UpdateAsync(TSale entity);
    Task<TSale> DeleteAsync(TSale entity);
    Task<List<TSale>> GetAllAsync();
    Task<TSale?> GetByIdAsync(Guid id);
    Task<IEnumerable<TSale>?> GetAllByUserPagedAsync(Guid userId, int page, int pageSize);
    Task<IEnumerable<TSale>?> GetAllDisputesByUserPagedAsync(Guid userId, int page, int pageSize);
}