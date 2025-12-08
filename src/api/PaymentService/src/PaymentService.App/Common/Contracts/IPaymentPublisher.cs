namespace Payments.App.Common.Contracts;

public interface IPaymentPublisher
{
    Task PublishPaymentSucceededAsync(Guid sourceId, Guid buyerId, DateTime expiresAt, decimal value, Guid paymentId, CancellationToken cancellationToken);
    Task PublishPaymentFailedAsync(Guid sourceId, Guid buyerId, DateTime expiresAt, CancellationToken cancellationToken);
}