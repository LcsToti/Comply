using MediatR;
using Payments.Domain.Aggregates.PaymentAggregate.Entities;
using Payments.Domain.Contracts;

namespace Payments.App.UseCases.PaymentCases.GetLastSuccessfulPaymentsCount;

public class GetLastSuccessfulPaymentsCountQueryHandler(IPaymentRepository<Payment> paymentRepository) : IRequestHandler<GetLastSuccessfulPaymentsCountQuery, int>
{
    private readonly IPaymentRepository<Payment> _paymentRepository = paymentRepository;
    
    public async Task<int> Handle(GetLastSuccessfulPaymentsCountQuery request, CancellationToken cancellationToken)
    {
        return await _paymentRepository.GetLastSuccessfulPaymentsCountAsync(request.amount, cancellationToken);
    }
}