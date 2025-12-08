using Payments.Domain.Exceptions.PaymentAccountExceptions;
using Payments.Domain.SeedWork;

namespace Payments.Domain.Aggregates.PaymentAccountAggregate.Entity;

public class PaymentAccount : IAggregateRoot
{
    public Guid UserId { get; private set; }
    public string CustomerId { get; private set; }
    public string ConnectedAccountId { get; private set; }
    public PaymentAccountStatus AccountStatus { get; private set; }
    
    internal PaymentAccount(Guid userId, string customerId, string connectedAccountId, PaymentAccountStatus accountStatus)
    {
        UserId = userId;
        CustomerId = customerId;
        ConnectedAccountId = connectedAccountId;
        AccountStatus = accountStatus;       
    }
}