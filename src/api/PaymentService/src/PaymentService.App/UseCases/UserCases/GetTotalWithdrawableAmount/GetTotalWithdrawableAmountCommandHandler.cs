using MediatR;
using Microsoft.Extensions.Logging;
using Payments.App.Common;
using Payments.App.Common.Errors;
using Payments.Domain.Aggregates.PaymentAccountAggregate.Entity;
using Payments.Domain.Aggregates.PaymentAggregate.Entities;
using Payments.Domain.Contracts;

namespace Payments.App.UseCases.UserCases.GetTotalWithdrawableAmount;

public class GetTotalWithdrawableAmountCommandHandler : IRequestHandler<GetTotalWithdrawableAmountCommand, Result<decimal>>
{
    private readonly IPaymentRepository<Payment> _paymentRepository;
    private readonly IPaymentAccountRepository<PaymentAccount> _paymentAccountRepository;   
    private readonly ILogger<GetTotalWithdrawableAmountCommandHandler> _logger;

    public GetTotalWithdrawableAmountCommandHandler(IPaymentRepository<Payment> paymentRepository, ILogger<GetTotalWithdrawableAmountCommandHandler> logger, IPaymentAccountRepository<PaymentAccount> paymentAccountRepository)
    {
        _paymentRepository = paymentRepository;
        _logger = logger;
        _paymentAccountRepository = paymentAccountRepository;       
    }
    public async Task<Result<decimal>> Handle(GetTotalWithdrawableAmountCommand request, CancellationToken cancellationToken)
    {
        var connectedAccountId = await _paymentAccountRepository.GetConnectedAccountIdByUserIdAsync(request.UserId, cancellationToken);
        if (connectedAccountId is null)
            return Result<decimal>.Failure(new NotFoundError(request.UserId, "No payment account found for user"));
        
        var payments = await _paymentRepository.GetApprovedPaymentsForWithdrawalBySellerAsync(request.UserId, cancellationToken);
        if (payments == null || payments.Count == 0)
        {
            _logger.LogWarning("Não há pagamentos com saque disponíveis para o vendedor.");
            return Result<decimal>.Success(0);
        }

        var totalWithdrawableAmount = payments.Sum(p => p.Amount.Net);
        
        return Result<decimal>.Success(totalWithdrawableAmount);
    }
}