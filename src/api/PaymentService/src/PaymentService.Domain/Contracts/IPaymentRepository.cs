using Payments.Domain.Aggregates.PaymentAggregate.Entities;
using Payments.Domain.SeedWork;

namespace Payments.Domain.Contracts
{
    public interface IPaymentRepository<T> where T : IAggregateRoot
    {
        Task AddAsync(Payment entity, CancellationToken cancellationToken);
        Task UpdateAsync(Payment entity, CancellationToken cancellationToken);
        Task DeleteAsync(string id, CancellationToken cancellationToken);
        Task<Payment?> GetByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken);
        Task<Payment?> GetPaymentByPayerId(Guid userId, Guid paymentId, CancellationToken cancellationToken);
        Task<List<Payment>?> GetAllByPayerAsync(Guid userId, CancellationToken cancellationToken);
        Task<List<Payment>?> GetApprovedPaymentsForWithdrawalBySellerAsync(Guid userId, CancellationToken cancellationToken);
        Task<bool> TryMarkAsWithdrawnAsync(Guid paymentId, CancellationToken cancellationToken);
        Task<bool> TryMarkAsWithdrawingAsync(Guid paymentId, CancellationToken cancellationToken);
        Task MarkAsWithdrawalFailedAsync(Guid paymentId, CancellationToken cancellationToken);
        Task<bool> ExistsBySourceIdAsync(Guid sourceId, CancellationToken cancellationToken);
        Task<int> GetLastSuccessfulPaymentsCountAsync(int amount, CancellationToken cancellationToken);
    }
}
