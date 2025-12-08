using MediatR;
using Microsoft.Extensions.Logging;
using Payments.App.Common;
using Payments.App.Common.Contracts;
using Payments.App.Common.Errors;
using Payments.App.Common.Results;
using Payments.App.Common.Results.Mappers;
using Payments.Domain.Aggregates.PaymentAccountAggregate.Entity;
using Payments.Domain.Aggregates.PaymentAggregate.Entities;
using Payments.Domain.Contracts;

namespace Payments.App.UseCases.UserCases.GetUserPaymentMethods;

public class GetUserPaymentMethodsCommandHandler : IRequestHandler<GetUserPaymentMethodsCommand, Result<IReadOnlyCollection<PaymentMethodResult>>>
{
    private readonly IPaymentGateway _paymentGateway;
    private readonly IPaymentAccountRepository<PaymentAccount> _paymentAccountRepository;   

    public GetUserPaymentMethodsCommandHandler(IPaymentGateway paymentGateway, IPaymentAccountRepository<PaymentAccount> paymentAccountRepository)
    {
        _paymentGateway = paymentGateway;
        _paymentAccountRepository = paymentAccountRepository;      
    }

    public async Task<Result<IReadOnlyCollection<PaymentMethodResult>>> Handle(GetUserPaymentMethodsCommand request, CancellationToken cancellationToken)
    {
        var customerId = await _paymentAccountRepository.GetCustomerIdByUserIdAsync(request.UserId, cancellationToken);
        if (customerId is null)
            return Result<IReadOnlyCollection<PaymentMethodResult>>.Failure(new NotFoundError(request.UserId, "No payment account found for user"));
        
        var paymentMethods = await _paymentGateway.GetPaymentMethodsAsync(customerId, cancellationToken);

        if (paymentMethods == Array.Empty<PaymentMethod>())
        {
            return Result<IReadOnlyCollection<PaymentMethodResult>>.Failure(new NotFoundError(request.UserId, "No payment methods found for user"));
        }

        var result = paymentMethods.Select(pm => pm.ToPaymentMethodResult()).ToArray();
        return Result<IReadOnlyCollection<PaymentMethodResult>>.Success(result);
    }
}