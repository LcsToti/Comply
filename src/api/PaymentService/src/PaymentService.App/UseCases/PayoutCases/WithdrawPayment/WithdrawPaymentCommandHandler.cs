using MediatR;
using Microsoft.Extensions.Logging;
using Payments.App.Common;
using Payments.App.Common.Contracts;
using Payments.App.Common.Errors;
using Payments.App.Common.Results;
using Payments.App.Common.Results.Mappers;
using Payments.App.Utils;
using Payments.Domain.Aggregate;
using Payments.Domain.Aggregates.PaymentAccountAggregate;
using Payments.Domain.Aggregates.PaymentAccountAggregate.Entity;
using Payments.Domain.Aggregates.PaymentAggregate.Entities;
using Payments.Domain.Aggregates.PaymentAggregate.Enums;
using Payments.Domain.Contracts;

namespace Payments.App.UseCases.PayoutCases.WithdrawPayment;

public class WithdrawPaymentCommandHandler : IRequestHandler<WithdrawPaymentCommand, Result<PaymentResult>>
{
    private readonly IPaymentGateway _paymentGateway;
    private readonly ILogger<WithdrawPaymentCommandHandler> _logger;
    private readonly IPaymentRepository<Payment> _paymentRepository;
    private readonly IPaymentAccountRepository<PaymentAccount> _paymentAccountRepository;   

    public WithdrawPaymentCommandHandler(
        IPaymentGateway paymentGateway, 
        ILogger<WithdrawPaymentCommandHandler> logger, 
        IPaymentRepository<Payment> paymentRepository, 
        IPaymentAccountRepository<PaymentAccount> paymentAccountRepository
        )
    {
        _paymentGateway = paymentGateway;
        _logger = logger;
        _paymentRepository = paymentRepository;
        _paymentAccountRepository = paymentAccountRepository;      
    }
    public async Task<Result<PaymentResult>> Handle(WithdrawPaymentCommand request, CancellationToken cancellationToken)
    {
        var sellerIdTask = _paymentAccountRepository.GetConnectedAccountIdByUserIdAsync(request.UserId, cancellationToken);
        var getPaymentTask = _paymentRepository.GetByPaymentIdAsync(request.PaymentId, cancellationToken);
        
        await Task.WhenAll(sellerIdTask, getPaymentTask);
        
        var connectedAccountId = sellerIdTask.Result;
        var payment = getPaymentTask.Result;       

        if (connectedAccountId is null)
        {
            return Result<PaymentResult>.Failure(new NotFoundError(request.UserId, "No payment account found for user"));
        }

        if (payment is null)
        {
            return Result<PaymentResult>.Failure(new NotFoundError(request.PaymentId, "Payment not found")); 
        }
        
        if (payment.SellerId == null)
        {
            _logger.LogWarning("Seller not found on payment {PaymentId}", request.PaymentId);
            return Result<PaymentResult>.Failure(new NotFoundError(connectedAccountId, $"Seller not found on payment {request.PaymentId}"));
        }
        
        if (payment.SellerId != request.UserId)
        {
            _logger.LogWarning("SECURITY ALERT: User {AuthenticatedUser} tried to withdraw payment {PaymentId} owned by {OwnerUser}.", 
                request.UserId, request.PaymentId, payment.SellerId);
            return Result<PaymentResult>.Failure(new Forbidden("You are not allowed to withdraw this payment."));
        }
        
        var accountStatus = await _paymentGateway.GetConnectedAccountStatusAsync(
            connectedAccountId, cancellationToken
        );

        if (accountStatus != PaymentAccountStatus.Active)
        {
            return Result<PaymentResult>.Failure(
                new InvalidPaymentOperation("Connected account is not enabled.")
            );
        }
        
        // Persist Transaction
        var locked = await _paymentRepository.TryMarkAsWithdrawingAsync(request.PaymentId, cancellationToken);
        if (!locked)
        {
            _logger.LogWarning("Payment {PaymentId} is already being withdrawn or not approved", request.PaymentId);
            return Result<PaymentResult>.Failure(new Conflict("Payment is already being withdrawn or not approved."));
        }
        
        // Domain validation
        payment.MarkAsWithdrawing();

        try
        {
            // Gateway transfer
            await _paymentGateway.TransferToConnectedAccountAsync(
                connectedAccountId,
                payment.Gateway.ApiChargeId,
                DecimalToLong.Convert(payment.Amount.Net),
                cancellationToken);

            // Domain validation
            payment.MarkAsWithdrawn();
            
            // Persist Transaction
            await _paymentRepository.TryMarkAsWithdrawnAsync(request.PaymentId, cancellationToken);
            
            _logger.LogInformation("Payment {PaymentId} successfully withdrawn", request.PaymentId);
            return Result<PaymentResult>.Success(payment.ToPaymentResult());
        }
        catch (Exception ex)
        {
            // Domain validation
            payment.MarkAsFailedToWithdraw();
            
            // Persist         
            await _paymentRepository.MarkAsWithdrawalFailedAsync(request.PaymentId, cancellationToken);
            
            _logger.LogError(ex, "Stripe transfer failed for payment {PaymentId}", request.PaymentId);
            return Result<PaymentResult>.Failure(new InvalidPaymentOperation("Stripe transfer failed."));
        }
    }
}