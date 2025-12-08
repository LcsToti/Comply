using SalesService.Domain.Common;

namespace SalesService.Domain.Aggregates.SaleAggregate.Exceptions;

public class InvalidDisputeException(string message) : DomainException(message);