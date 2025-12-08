using MediatR;
using Microsoft.Extensions.Logging;
using Payments.App.Common.Contracts;
using Payments.Domain.Aggregates.PaymentAccountAggregate;
using Payments.Domain.Aggregates.PaymentAccountAggregate.Entity;
using Payments.Domain.Aggregates.PaymentAccountAggregate.Factories;
using Payments.Domain.Contracts;

namespace Payments.App.UseCases.UserCases.CreatePaymentAccount;

public class CreatePaymentAccountEventHandler : IRequestHandler<CreatePaymentAccountEvent>
{
    private readonly IPaymentGateway _paymentGateway;
    private readonly ILogger<CreatePaymentAccountEventHandler> _logger;
    private readonly IPaymentAccountRepository<PaymentAccount> _paymentAccountRepository;

    public CreatePaymentAccountEventHandler(IPaymentGateway paymentGateway, ILogger<CreatePaymentAccountEventHandler> logger, IPaymentAccountRepository<PaymentAccount> paymentAccountRepository)
    {
        _paymentGateway = paymentGateway;
        _logger = logger;
        _paymentAccountRepository = paymentAccountRepository;
    }
    public async Task Handle(CreatePaymentAccountEvent request, CancellationToken cancellationToken)
    {
        var customerTask = _paymentGateway.CreateCustomerAsync(request.Email, request.Name, cancellationToken);
        var connectedAccountTask = _paymentGateway.CreateConnectedAccountAsync(request.Email, cancellationToken);
        
        // Gateway
        await Task.WhenAll(customerTask, connectedAccountTask);
        
        var customerId = await customerTask;
        var connectedAccountId = await connectedAccountTask;
        
            
        // Domain
        var paymentAccount = PaymentAccountFactory.Create(request.UserId, customerId, connectedAccountId, PaymentAccountStatus.None);
        
        // Persist
        await _paymentAccountRepository.AddAsync(paymentAccount, cancellationToken);
        _logger.LogInformation("Payment account fully created for user {Name}", request.Name);
    }
}