using MediatR;
using Payments.App.Common;
using Payments.App.Common.Errors;
using Payments.App.Common.Results;
using Payments.App.Common.Results.Mappers;
using Payments.Domain.Aggregates.PaymentAccountAggregate.Entity;
using Payments.Domain.Aggregates.PaymentAggregate.Entities;
using Payments.Domain.Contracts;

namespace Payments.App.UseCases.PaymentCases.GetPayment
{
    public class GetPaymentCommandHandler : IRequestHandler<GetPaymentCommand, Result<PaymentResult>>
    {
        private readonly IPaymentRepository<Payment> _paymentRepository;
        private readonly IPaymentAccountRepository<PaymentAccount> _paymentAccountRepository;

        public GetPaymentCommandHandler(IPaymentRepository<Payment> paymentRepository, IPaymentAccountRepository<PaymentAccount> paymentAccountRepository)
        {
            _paymentRepository = paymentRepository;
            _paymentAccountRepository = paymentAccountRepository;
        }

        public async Task<Result<PaymentResult>> Handle(GetPaymentCommand request, CancellationToken cancellationToken)
        {
            var payment = await _paymentRepository.GetByPaymentIdAsync(request.PaymentId, cancellationToken);
            
            if (payment == null)
            {
                return Result<PaymentResult>.Failure(new NotFoundError(request.PaymentId, "Payment not found"));
            }

            if (request.Role is "Admin" or "Moderator" || payment.PayerId == request.UserId || payment.SellerId == request.UserId)
            {
                return Result<PaymentResult>.Success(payment.ToPaymentResult());
            }

            return Result<PaymentResult>.Failure(new Forbidden("You are not allowed to view this payment."));           
        }
    }
}
