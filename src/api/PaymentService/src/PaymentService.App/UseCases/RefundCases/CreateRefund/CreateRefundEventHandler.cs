using MediatR;
using Microsoft.Extensions.Logging;
using Payments.App.Common.Contracts;
using Payments.App.Utils;
using Payments.Domain.Aggregates.PaymentAccountAggregate.Entity;
using Payments.Domain.Aggregates.PaymentAggregate.Entities;
using Payments.Domain.Contracts;

namespace Payments.App.UseCases.RefundCases.CreateRefund
{
    public class CreateRefundEventHandler : IRequestHandler<CreateRefundEvent>
    {
        private readonly IPaymentGateway _paymentGateway;
        private readonly IPaymentRepository<Payment> _paymentRepository;
        private readonly IPaymentAccountRepository<PaymentAccount> _paymentAccountRepository;
        private readonly ILogger<CreateRefundEventHandler> _logger;
        private readonly IPaymentPublisher _paymentPublisher;
        public CreateRefundEventHandler(
            IPaymentGateway paymentGateway, 
            IPaymentRepository<Payment> paymentRepository, 
            ILogger<CreateRefundEventHandler> logger,
            IPaymentAccountRepository<PaymentAccount> paymentAccountRepository,
            IPaymentPublisher paymentPublisher
            )
        {
            _paymentGateway = paymentGateway;
            _paymentRepository = paymentRepository;
            _logger = logger;
            _paymentAccountRepository = paymentAccountRepository;
            _paymentPublisher = paymentPublisher;
        }

        public async Task Handle(CreateRefundEvent request, CancellationToken cancellationToken)
        {
            var getCustomerTask = _paymentAccountRepository.GetCustomerIdByUserIdAsync(request.UserId, cancellationToken);
            var getPaymentToBeRefundedTask = _paymentRepository.GetByPaymentIdAsync(request.PaymentId, cancellationToken);
            
            await Task.WhenAll(getCustomerTask, getPaymentToBeRefundedTask);
            
            var customerId = getCustomerTask.Result;
            var paymentToBeRefunded = getPaymentToBeRefundedTask.Result;

            if (customerId is null)
            {
                _logger.LogWarning("Customer not found for id {userId}", request.UserId);
                return;
            }
            
            if (paymentToBeRefunded == null)
            {
                _logger.LogWarning("Payment not found for id {paymentId}", request.PaymentId);
                return;
            }
            
            // Domain Validation
            paymentToBeRefunded.EnsureCanBeRefunded(request.Amount);
            
            // Gateway
            var processRefund = await _paymentGateway.RefundPaymentIntentAsync(paymentToBeRefunded.Gateway.ApiPaymentId, DecimalToLong.Convert(request.Amount), request.Reason, cancellationToken);
            
            // Domain Validation
            paymentToBeRefunded.AddRefund(processRefund);

            // Publish
            
            // Persist
            await _paymentRepository.UpdateAsync(paymentToBeRefunded, cancellationToken);
            _logger.LogInformation("Payment {paymentId} refunded", request.PaymentId);
        }
    }
}
