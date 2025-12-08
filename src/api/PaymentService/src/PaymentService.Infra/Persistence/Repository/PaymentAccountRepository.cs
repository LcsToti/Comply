using MongoDB.Driver;
using Payments.Domain.Aggregates.PaymentAccountAggregate;
using Payments.Domain.Aggregates.PaymentAccountAggregate.Entity;
using Payments.Domain.Contracts;
using Payments.Domain.SeedWork;
using Payments.Infra.Persistence.DataModel;
using Payments.Infra.Persistence.Mappers;

namespace Payments.Infra.Persistence.Repository;

public class PaymentAccountRepository<T> : IPaymentAccountRepository<T> where T : IAggregateRoot
{
    private readonly IMongoCollection<PaymentAccountDataModel> _collection;
    
    public PaymentAccountRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<PaymentAccountDataModel>("PaymentAccounts");
    }
    public async Task AddAsync(PaymentAccount paymentAccount, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(paymentAccount);
        var dataModel = PaymentAccountMapper.ToDataModel(paymentAccount);
        
        await _collection.InsertOneAsync(dataModel, null, cancellationToken);
    }

    public async Task UpdateAsync(PaymentAccount paymentAccount, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(paymentAccount);
        var dataModel = PaymentAccountMapper.ToDataModel(paymentAccount);
        await _collection.ReplaceOneAsync(p => p.UserId == paymentAccount.UserId, dataModel, cancellationToken: cancellationToken);
    }

    public async Task<string?> GetCustomerIdByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    { 
        var dataModel = await _collection.Find(p => p.UserId == userId).FirstOrDefaultAsync(cancellationToken);
        return dataModel?.CustomerId;
    }

    public async Task<string?> GetConnectedAccountIdByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var dataModel = await _collection.Find(p => p.UserId == userId).FirstOrDefaultAsync(cancellationToken);
        return dataModel?.ConnectedAccountId;       
    }

    public async Task UpdatePayoutStatusAsync(Guid userId, PaymentAccountStatus accountStatus, CancellationToken cancellationToken)
    {
        var update = Builders<PaymentAccountDataModel>.Update.Set(p => p.AccountStatus, accountStatus);
        await _collection.UpdateOneAsync(p => p.UserId == userId, update, cancellationToken: cancellationToken);
    }

}