using MediatR;

namespace Payments.App.UseCases.PaymentCases.GetLastSuccessfulPaymentsCount;

public record GetLastSuccessfulPaymentsCountQuery(int amount) : IRequest<int>;