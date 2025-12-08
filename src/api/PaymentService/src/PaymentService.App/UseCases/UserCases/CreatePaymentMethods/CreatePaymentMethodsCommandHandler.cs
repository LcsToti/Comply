using MediatR;
using Microsoft.Extensions.Logging;
using Payments.App.Common;
using Payments.App.Common.Contracts;
using Payments.App.Common.Errors;
using Payments.Domain.Aggregates.PaymentAccountAggregate.Entity;
using Payments.Domain.Contracts;

namespace Payments.App.UseCases.UserCases.CreatePaymentMethods;

public class CreatePaymentMethodsCommandHandler : IRequestHandler<CreatePaymentMethodsCommand, Result<string>>
{
    private readonly IPaymentGateway _paymentGateway;
    private readonly IPaymentAccountRepository<PaymentAccount> _paymentAccountRepository;
    private readonly ILogger<CreatePaymentMethodsCommandHandler> _logger;

    public CreatePaymentMethodsCommandHandler(IPaymentGateway paymentGateway, ILogger<CreatePaymentMethodsCommandHandler> logger, IPaymentAccountRepository<PaymentAccount> paymentAccountRepository)
    {
        _paymentGateway = paymentGateway;
        _logger = logger;
        _paymentAccountRepository = paymentAccountRepository;
    }
    
    public async Task<Result<string>> Handle(CreatePaymentMethodsCommand request, CancellationToken cancellationToken)
    {
        var customerId = await _paymentAccountRepository.GetCustomerIdByUserIdAsync(request.UserId, cancellationToken);
        if (customerId is null)
            return Result<string>.Failure(new NotFoundError(request.UserId, "No payment account found for user"));
        
        
        _logger.LogInformation("Creating payment methods for customer {customerId}", customerId);
        // Gateway
        var paymentMethodId = await _paymentGateway.CreateSetupIntentAsync(customerId, cancellationToken);
        
        _logger.LogInformation("Payment methods created for customer {customerId}", customerId);
        return Result<string>.Success(paymentMethodId);
    }
}