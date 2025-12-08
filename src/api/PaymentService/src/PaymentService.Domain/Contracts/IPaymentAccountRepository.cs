using Payments.Domain.Aggregates.PaymentAccountAggregate;
using Payments.Domain.Aggregates.PaymentAccountAggregate.Entity;
using Payments.Domain.SeedWork;

namespace Payments.Domain.Contracts;

public interface IPaymentAccountRepository<T> where T : IAggregateRoot
{
    Task AddAsync(PaymentAccount paymentAccount, CancellationToken cancellationToken);
    Task UpdateAsync(PaymentAccount paymentAccount, CancellationToken cancellationToken);
    Task<string?> GetCustomerIdByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<string?> GetConnectedAccountIdByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task UpdatePayoutStatusAsync(Guid userId, PaymentAccountStatus accountStatus, CancellationToken cancellationToken);
}