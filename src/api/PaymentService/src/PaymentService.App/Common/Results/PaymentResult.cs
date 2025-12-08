using Payments.Domain.Aggregate.VOs;
using Payments.Domain.Aggregates.PaymentAggregate.VOs;

namespace Payments.App.Common.Results;

public record PaymentResult(
    Guid PaymentId,
    string WithdrawalStatus,
    string PaymentStatus,
    Guid PayerId,
    Amount Amount,
    Timestamps Timestamps
    );