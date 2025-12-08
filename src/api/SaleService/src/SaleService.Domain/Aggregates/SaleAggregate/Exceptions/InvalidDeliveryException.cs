using SalesService.Domain.Common;

namespace SalesService.Domain.Aggregates.SaleAggregate.Exceptions;

public class InvalidDeliveryException(string message) : DomainException(message);