using Payments.Domain.Contracts;
using Payments.Domain.SeedWork;
using MongoDB.Driver;
using Payments.Domain.Aggregates.PaymentAggregate.Entities;
using Payments.Domain.Aggregates.PaymentAggregate.Enums;
using Payments.Infra.Persistence.DataModel;
using Payments.Infra.Persistence.Mappers;

namespace Payments.Infra.Persistence.Repository
{
    public class PaymentRepository<T> : IPaymentRepository<T> where T : IAggregateRoot
    {
        private readonly IMongoCollection<PaymentDataModel> _collection;

        public PaymentRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<PaymentDataModel>("Payments");
        }

        public async Task<Payment?> GetByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken)
        {
            var dataModel = await _collection.Find(p => p.Id == paymentId).FirstOrDefaultAsync(cancellationToken);

            return dataModel is not null ? PaymentMapper.ToDomain(dataModel) : null;
        }

        public async Task<Payment?> GetPaymentByPayerId(Guid userId, Guid paymentId,
            CancellationToken cancellationToken)
        {
            var dataModel = await _collection.Find(p => p.Id == paymentId && p.PayerId == userId)
                .FirstOrDefaultAsync(cancellationToken);

            return dataModel is not null ? PaymentMapper.ToDomain(dataModel) : null;
        }

        public async Task<List<Payment>?> GetAllByPayerAsync(Guid userId, CancellationToken cancellationToken)
        {
            var dataModels = await _collection.Find(p => p.PayerId == userId).ToListAsync(cancellationToken);

            var payments = dataModels.Select(PaymentMapper.ToDomain).ToList();

            return payments;
        }

        public async Task<List<Payment>?> GetApprovedPaymentsForWithdrawalBySellerAsync(Guid userId, CancellationToken cancellationToken)
        {
            var dataModels = await _collection.Find(p =>
                p.SellerId == userId &&
                (p.WithdrawalStatus == WithdrawalStatus.ApprovedToWithdraw ||
                 p.WithdrawalStatus == WithdrawalStatus.Failed)
            ).ToListAsync(cancellationToken);

            return dataModels.Select(PaymentMapper.ToDomain).ToList();
        }

        

        public async Task AddAsync(Payment entity, CancellationToken cancellationToken)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            var dataModel = PaymentMapper.ToDataModel(entity);
            await _collection.InsertOneAsync(dataModel, null, cancellationToken);
        }

        public async Task UpdateAsync(Payment entity, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(entity);

            var dataModel = PaymentMapper.ToDataModel(entity);
            await _collection.ReplaceOneAsync(p => p.Id == entity.Id, dataModel, cancellationToken: cancellationToken);
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken)
        {
            await _collection.DeleteOneAsync(p => p.Id.ToString() == id, cancellationToken);
        }
        
        public async Task<bool> TryMarkAsWithdrawnAsync(Guid paymentId, CancellationToken cancellationToken)
        {
            var filter = Builders<PaymentDataModel>.Filter.And(
                Builders<PaymentDataModel>.Filter.Eq(p => p.Id, paymentId),
                Builders<PaymentDataModel>.Filter.Eq(p => p.WithdrawalStatus, WithdrawalStatus.Withdrawing)
            );
            
            var update = Builders<PaymentDataModel>.Update
                .Set(p => p.WithdrawalStatus, WithdrawalStatus.AlreadyWithdrawn)
                .Set(p => p.Timestamps.UpdatedAt, DateTime.UtcNow)
                .Set(p => p.Timestamps.WithdrawnAt, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

            return result.ModifiedCount > 0;
        }
        
        public async Task<bool> TryMarkAsWithdrawingAsync(Guid paymentId, CancellationToken cancellationToken)
        {
            var filter = Builders<PaymentDataModel>.Filter.And(
                Builders<PaymentDataModel>.Filter.Eq(p => p.Id, paymentId),
                Builders<PaymentDataModel>.Filter.In(p => p.WithdrawalStatus, new[]
                {
                    WithdrawalStatus.ApprovedToWithdraw,
                    WithdrawalStatus.Failed
                })
            );

            var update = Builders<PaymentDataModel>.Update
                .Set(p => p.WithdrawalStatus, WithdrawalStatus.Withdrawing)
                .Set(p => p.Timestamps.UpdatedAt, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

            return result.ModifiedCount > 0;
        }
        
        public async Task MarkAsWithdrawalFailedAsync(Guid paymentId, CancellationToken cancellationToken)
        {
            var filter = Builders<PaymentDataModel>.Filter.Eq<Guid>(p => p.Id, paymentId);

            var update = Builders<PaymentDataModel>.Update
                .Set(p => p.WithdrawalStatus, WithdrawalStatus.Failed)
                .Set(p => p.Timestamps.UpdatedAt, DateTime.UtcNow);

            await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        }

        public async Task<bool> ExistsBySourceIdAsync(Guid sourceId, CancellationToken cancellationToken)
        {
            return await _collection.Find(p => p.SourceId == sourceId)
                .Limit(1)
                .AnyAsync(cancellationToken);
        }

        public async Task<int> GetLastSuccessfulPaymentsCountAsync(int amount, CancellationToken cancellationToken)
        {
            var successfulFilter = Builders<PaymentDataModel>.Filter
                .Eq(p => p.Status, PaymentStatus.Succeeded);

            var limitedPayments = await _collection
                .Find(successfulFilter)
                .SortByDescending(p => p.Timestamps.CreatedAt)
                .Limit(amount)
                .ToListAsync(cancellationToken);

            return limitedPayments.Count;
        }
    }
}
