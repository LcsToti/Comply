using MediatR;
using Microsoft.Extensions.Logging;
using Payments.App.Common;
using Payments.App.Common.Contracts;
using Payments.App.Common.Errors;
using Payments.Domain.Aggregates.PaymentAccountAggregate.Entity;
using Payments.Domain.Contracts;

namespace Payments.App.UseCases.UserCases.GetUserOnboardingLink;

public class GetUserOnboardingLinkCommandHandler : IRequestHandler<GetUserOnboardingLinkCommand, Result<string>>
{
    private readonly IPaymentAccountRepository<PaymentAccount> _paymentAccountRepository;   
    private readonly ILogger<GetUserOnboardingLinkCommandHandler> _logger;
    private readonly IPaymentGateway _paymentGateway;
    
    public GetUserOnboardingLinkCommandHandler(IPaymentAccountRepository<PaymentAccount> paymentAccountRepository, ILogger<GetUserOnboardingLinkCommandHandler> logger, IPaymentGateway paymentGateway)
    {
        _paymentAccountRepository = paymentAccountRepository; 
        _logger = logger;
        _paymentGateway = paymentGateway;
        
    }
    
    public async Task<Result<string>> Handle(GetUserOnboardingLinkCommand request, CancellationToken cancellationToken)
    {
        var connectedAccountId = await _paymentAccountRepository.GetConnectedAccountIdByUserIdAsync(request.UserId, cancellationToken);
        if (connectedAccountId is not null)
        {
            var onboardingLink = await _paymentGateway.CreateAccountLink(connectedAccountId, cancellationToken);
            _logger.LogInformation("Onboarding link generated for user {UserId}", request.UserId);
            return Result<string>.Success(onboardingLink);
        }
        
        return Result<string>.Failure(new NotFoundError(request.UserId, "No payment account found for user"));
    }
}