using MediatR;
using Microsoft.Extensions.Logging;
using Payments.App.Common.Contracts;
using Payments.App.UseCases.RefundCases.CreateRefund;
using Payments.App.Utils;
using Payments.Domain.Aggregates.PaymentAccountAggregate.Entity;
using Payments.Domain.Aggregates.PaymentAggregate.Entities;
using Payments.Domain.Aggregates.PaymentAggregate.Enums;
using Payments.Domain.Contracts;

namespace Payments.App.UseCases.PaymentCases.CreatePayment;
public class CreatePaymentEventHandler : IRequestHandler<CreatePaymentEvent>
{
    private readonly IPaymentGateway _paymentGateway;
    private readonly IPaymentRepository<Payment> _paymentRepository;
    private readonly ILogger<CreatePaymentEventHandler> _logger;
    private readonly IPaymentAccountRepository<PaymentAccount> _paymentAccountRepository;
    private readonly IMediator _mediator;

    public CreatePaymentEventHandler(
        IPaymentGateway paymentGateway,
        IPaymentRepository<Payment> paymentRepository,
        IPaymentPublisher paymentPublisher,
        ILogger<CreatePaymentEventHandler> logger,
        IPaymentAccountRepository<PaymentAccount> paymentAccountRepository,
        IMediator mediator
        )
    {
        _paymentGateway = paymentGateway;
        _paymentRepository = paymentRepository;
        _logger = logger;
        _paymentAccountRepository = paymentAccountRepository;
        _mediator = mediator;
    }

    public async Task Handle(CreatePaymentEvent request, CancellationToken cancellationToken)
    {
        var customerId = await _paymentAccountRepository.GetCustomerIdByUserIdAsync(request.BuyerId, cancellationToken);

        if (customerId is null)
        {
            _logger.LogWarning("No customer found for user {UserId} — skipping payment creation.", request.BuyerId);
            return;
        }

        if (DateTime.UtcNow >= request.ExpiresAt)
        {
            _logger.LogWarning("Payment {SourceId} expired before processing.", request.SourceId);
            // Publish
            await request.Publisher.PublishPaymentFailedAsync(request.SourceId, request.BuyerId, request.ExpiresAt, cancellationToken);
            return;
        }

        try
        {
            // Gateway
            var payment = await _paymentGateway.CreatePaymentIntentAsync(
                DecimalToLong.Convert(request.Value),
                customerId,
                request.PaymentMethod,
                cancellationToken
            );
            
            if (DateTime.UtcNow >= request.ExpiresAt)
            {
                _logger.LogWarning("Payment {SourceId} expired during processing — creating refund.", request.SourceId);
                // App create refund
                await _mediator.Send(new CreateRefundEvent(request.Value, payment.Id, "Payment time limit exceeded — please start a new transaction.", request.BuyerId, request.Publisher), cancellationToken);
                return;
            }

            // Domain Validation
            payment.AddSource(request.SourceId);
            payment.AddPayer(request.BuyerId);
            payment.AddSeller(request.SellerId);

            // Persist
            var persistTask = _paymentRepository.AddAsync(payment, cancellationToken);

            // Publish
            var brokerTask = request.Publisher.PublishPaymentSucceededAsync(
                sourceId: request.SourceId,
                buyerId: request.BuyerId,
                expiresAt: request.ExpiresAt,
                value: request.Value,
                paymentId: payment.Id,
                cancellationToken);

            await Task.WhenAll(persistTask, brokerTask);
            _logger.LogInformation("Payment {SourceId} successfully processed and published for user {UserId}.", request.SourceId, request.BuyerId);
        }
        catch
        {
            _logger.LogWarning("Payment {SourceId} failed during processing.", request.SourceId);
            // Publish
            await request.Publisher.PublishPaymentFailedAsync(request.SourceId, request.BuyerId, request.ExpiresAt,
                cancellationToken);
            _logger.LogWarning("Published payment failed for user {UserId}.", request.BuyerId);
        }
    }
}