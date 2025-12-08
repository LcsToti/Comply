using MediatR;
using Microsoft.Extensions.Logging;
using Payments.App.Common;
using Payments.App.Common.Errors;
using Payments.App.Common.Results;
using Payments.App.Common.Results.Mappers;
using Payments.App.UseCases.PayoutCases.WithdrawPayment;
using Payments.Domain.Aggregates.PaymentAccountAggregate.Entity;
using Payments.Domain.Aggregates.PaymentAggregate.Entities;
using Payments.Domain.Contracts;

namespace Payments.App.UseCases.PayoutCases.WithdrawAllAvailablePayments;

public class WithdrawAllAvailablePaymentsCommandHandler : IRequestHandler<WithdrawAllAvailablePaymentsCommand, Result<PaymentResult[]>>
{
    private readonly ILogger<WithdrawAllAvailablePaymentsCommandHandler> _logger;
    private readonly IPaymentRepository<Payment> _paymentRepository;
    private readonly IPaymentAccountRepository<PaymentAccount> _paymentAccountRepository;
    private readonly IMediator _mediator;
    
    public WithdrawAllAvailablePaymentsCommandHandler(
        ILogger<WithdrawAllAvailablePaymentsCommandHandler> logger, 
        IPaymentRepository<Payment> paymentRepository, 
        IMediator mediator, 
        IPaymentAccountRepository<PaymentAccount> paymentAccountRepository)
    {
        _logger = logger;
        _paymentRepository = paymentRepository;
        _mediator = mediator;
        _paymentAccountRepository = paymentAccountRepository;
    }
    
    public async Task<Result<PaymentResult[]>> Handle(WithdrawAllAvailablePaymentsCommand request, CancellationToken cancellationToken)
    {
        
        var payments = await _paymentRepository.GetApprovedPaymentsForWithdrawalBySellerAsync(request.UserId, cancellationToken);
        if (payments == null || payments.Count == 0)
        {
            return Result<PaymentResult[]>.Failure(new NotFoundError(request.UserId, $"No payments approved to withdraw was found for seller"));
        }
        
        var throttler = new SemaphoreSlim(5);
        
        var tasks = payments.Select(async payment =>
        {
            await throttler.WaitAsync(cancellationToken);
            try
            {
                var result = await _mediator.Send(
                    new WithdrawPaymentCommand(payment.Id, request.UserId),
                    cancellationToken);

                return (Payment: payment, Result: result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to withdraw payment {PaymentId} for user {UserId}",
                    payment.Id, request.UserId);

                return (Payment: payment, Result: Result<PaymentResult>.Failure(
                        new InvalidPaymentOperation($"Withdrawal failed for {payment.Id}")));
            }
            finally
            {
                throttler.Release();
            }
        });
        
        var processed = await Task.WhenAll(tasks);
        
        var successes = processed
            .Where(x => x.Result.IsSuccess)
            .Select(x => x.Result.Value)
            .ToArray();

        if (successes.Length == 0)
            return Result<PaymentResult[]>.Failure(new InvalidPaymentOperation("All withdrawals failed."));

        return Result<PaymentResult[]>.Success(successes);
    }
}