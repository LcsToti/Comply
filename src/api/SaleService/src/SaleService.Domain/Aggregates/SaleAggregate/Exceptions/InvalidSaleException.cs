using SalesService.Domain.Common;

namespace SalesService.Domain.Aggregates.SaleAggregate.Exceptions;

public class InvalidSaleException(string message) : DomainException(message);