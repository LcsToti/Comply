using MediatR;
using Payments.App.Common;
using Payments.App.Common.Contracts;
using Payments.App.Common.Errors;
using Payments.App.Common.Results;
using Payments.App.Common.Results.Mappers;
using Payments.Domain.Aggregates.PaymentAccountAggregate.Entity;
using Payments.Domain.Aggregates.PaymentAggregate.Entities;
using Payments.Domain.Contracts;

namespace Payments.App.UseCases.PaymentCases.GetUserPayments
{
    public class GetPayerPaymentsCommandHandler : IRequestHandler<GetPayerPaymentsCommand, Result<PaymentResult[]>>
    {
        private readonly IPaymentRepository<Payment> _paymentRepository;
        private readonly IPaymentAccountRepository<PaymentAccount> _paymentAccountRepository;
        public GetPayerPaymentsCommandHandler(IPaymentGateway paymentGateway, IPaymentRepository<Payment> paymentRepository, IPaymentAccountRepository<PaymentAccount> paymentAccountRepository)
        {
            _paymentRepository = paymentRepository;
            _paymentAccountRepository = paymentAccountRepository;
        }

        public async Task<Result<PaymentResult[]>> Handle(GetPayerPaymentsCommand request, CancellationToken cancellationToken)
        {
            var payments = await _paymentRepository.GetAllByPayerAsync(request.UserId, cancellationToken);

            if (payments == null || payments.Count == 0)
            {
                return Result<PaymentResult[]>.Failure(new NotFoundError(request.UserId, "No payments found for user"));
            }

            var results = payments.Select(p => p.ToPaymentResult()).ToArray();
            return Result<PaymentResult[]>.Success(results);
        }
    }
}
