namespace SalesService.Domain.Aggregates.SaleAggregate.Enums;

public enum DisputeResolutionStatus
{
    Solved,
    Unsolved,
    Refunded,
    ApprovedWithdrawal,
    ResolvedByAdmin,
    Expired
}