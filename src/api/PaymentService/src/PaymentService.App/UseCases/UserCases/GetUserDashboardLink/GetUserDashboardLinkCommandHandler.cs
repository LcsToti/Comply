using MediatR;
using Microsoft.Extensions.Logging;
using Payments.App.Common;
using Payments.App.Common.Contracts;
using Payments.App.Common.Errors;
using Payments.Domain.Aggregates.PaymentAccountAggregate.Entity;
using Payments.Domain.Contracts;

namespace Payments.App.UseCases.UserCases.GetUserDashboardLink;

public class GetUserDashboardLinkCommandHandler : IRequestHandler<GetUserDashboardLinkCommand, Result<string>>
{
    private readonly IPaymentAccountRepository<PaymentAccount> _paymentAccountRepository;   
    private readonly ILogger<GetUserDashboardLinkCommandHandler> _logger;
    private readonly IPaymentGateway _paymentGateway;
    
    public GetUserDashboardLinkCommandHandler(IPaymentAccountRepository<PaymentAccount> paymentAccountRepository, ILogger<GetUserDashboardLinkCommandHandler> logger, IPaymentGateway paymentGateway)
    {
        _paymentAccountRepository = paymentAccountRepository; 
        _logger = logger;
        _paymentGateway = paymentGateway;
        
    }
    
    public async Task<Result<string>> Handle(GetUserDashboardLinkCommand request, CancellationToken cancellationToken)
    {
        var connectedAccountId = await _paymentAccountRepository.GetConnectedAccountIdByUserIdAsync(request.UserId, cancellationToken);
        if (connectedAccountId is not null)
        {
            var dashboardLink = await _paymentGateway.CreateDashboardLink(connectedAccountId, cancellationToken);
            _logger.LogInformation("Dashboard link generated for user {UserId}", request.UserId);
            return Result<string>.Success(dashboardLink);
        }
        
        return Result<string>.Failure(new NotFoundError(request.UserId, "No payment account found for user"));
    }
}