using MediatR;
using Microsoft.Extensions.Logging;
using Payments.App.Common;
using Payments.App.Common.Contracts;
using Payments.App.Common.Errors;
using Payments.Domain.Aggregates.PaymentAccountAggregate;
using Payments.Domain.Aggregates.PaymentAccountAggregate.Entity;
using Payments.Domain.Contracts;

namespace Payments.App.UseCases.UserCases.GetConnectedAccountStatus;

public class GetConnectedAccountStatusCommandHandler : IRequestHandler<GetConnectedAccountStatusCommand, Result<PaymentAccountStatus>>
{
    private readonly IPaymentGateway _paymentGateway;
    private readonly IPaymentAccountRepository<PaymentAccount> _paymentAccountRepository;

    public GetConnectedAccountStatusCommandHandler(IPaymentGateway paymentGateway,
        IPaymentAccountRepository<PaymentAccount> paymentAccountRepository)
    {
        _paymentGateway = paymentGateway;
        _paymentAccountRepository = paymentAccountRepository;      
    }
    
    public async Task<Result<PaymentAccountStatus>> Handle(GetConnectedAccountStatusCommand request, CancellationToken cancellationToken)
    {
        var connectedAccountId = await _paymentAccountRepository.GetConnectedAccountIdByUserIdAsync(request.UserId, cancellationToken);

        if (connectedAccountId is null)
        {
            return Result<PaymentAccountStatus>.Failure(new NotFoundError(request.UserId, "No payment account found for user"));
        }
        
        var accountStatus = await _paymentGateway.GetConnectedAccountStatusAsync(connectedAccountId, cancellationToken);
        
        await _paymentAccountRepository.UpdatePayoutStatusAsync(request.UserId, accountStatus, cancellationToken);
        return Result<PaymentAccountStatus>.Success(accountStatus);
    }
}