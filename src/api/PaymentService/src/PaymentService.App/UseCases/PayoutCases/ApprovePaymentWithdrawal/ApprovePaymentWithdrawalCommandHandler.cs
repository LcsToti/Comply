using MediatR;
using Microsoft.Extensions.Logging;
using Payments.App.Common;
using Payments.App.Common.Errors;
using Payments.App.Common.Results;
using Payments.App.Common.Results.Mappers;
using Payments.Domain.Aggregates.PaymentAggregate.Entities;
using Payments.Domain.Contracts;

namespace Payments.App.UseCases.PayoutCases.ApprovePaymentWithdrawal;

public class ApprovePaymentWithdrawalCommandHandler : IRequestHandler<ApprovePaymentWithdrawalCommand, Result<PaymentResult>>
{
    private readonly IPaymentRepository<Payment> _paymentRepository;
    private readonly ILogger<ApprovePaymentWithdrawalCommandHandler> _logger;
    
    public ApprovePaymentWithdrawalCommandHandler(IPaymentRepository<Payment> paymentRepository, ILogger<ApprovePaymentWithdrawalCommandHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _logger = logger;
    }
    
    public async Task<Result<PaymentResult>> Handle(ApprovePaymentWithdrawalCommand request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByPaymentIdAsync(request.PaymentId, cancellationToken);
        if (payment == null)
        {
            _logger.LogWarning("Payment not found for id {paymentId}", request.PaymentId);
            return Result<PaymentResult>.Failure(new NotFoundError(request.PaymentId, "Payment not found"));
        }
        
        payment.MarkAsApprovedToWithdraw();
        _logger.LogInformation("Payment {paymentId} approved to withdraw", request.PaymentId);
        await _paymentRepository.UpdateAsync(payment, cancellationToken);

        var result = payment.ToPaymentResult();
        return Result<PaymentResult>.Success(result);
    }
}