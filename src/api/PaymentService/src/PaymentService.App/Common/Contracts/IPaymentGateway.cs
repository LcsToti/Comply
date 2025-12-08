using Payments.Domain.Aggregates.PaymentAccountAggregate;
using Payments.Domain.Aggregates.PaymentAggregate.Entities;

namespace Payments.App.Common.Contracts
{
    public interface IPaymentGateway
    {
        #region Payments
        Task<Payment> CreatePaymentIntentAsync(long amount, string customerId, string paymentMethodId, CancellationToken cancellationToken);
        Task<Refund> RefundPaymentIntentAsync(string apiPaymentId, long amount, string reason, CancellationToken cancellationToken);
        #endregion
        
        #region Customer
        Task<string> CreateCustomerAsync(string email, string name, CancellationToken cancellationToken);
        #endregion
        
        #region ConnectedAccount
        Task<string> CreateConnectedAccountAsync(string email, CancellationToken cancellationToken);
        Task<string> CreateAccountLink(string connectedAccountId, CancellationToken cancellationToken);
        Task<string> CreateDashboardLink(string connectedAccountId, CancellationToken cancellationToken);
        Task TransferToConnectedAccountAsync(string connectedAccountId, string apiChargeId, long amount, CancellationToken cancellationToken);
        Task<PaymentAccountStatus> GetConnectedAccountStatusAsync(string connectedAccountId, CancellationToken cancellationToken);
        #endregion
        
        #region PaymentMethods
        Task<string> CreateSetupIntentAsync(string customerId, CancellationToken cancellationToken);
        Task<IReadOnlyCollection<PaymentMethod>> GetPaymentMethodsAsync(string customerId, CancellationToken cancellationToken);
        #endregion
    }
}
