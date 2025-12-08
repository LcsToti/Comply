using SalesService.Domain.Aggregates.SaleAggregate.Enums;

namespace SalesService.API.Requests;

public record CloseDisputeRequest(string Resolution, DisputeResolutionStatus ResolutionStatus);