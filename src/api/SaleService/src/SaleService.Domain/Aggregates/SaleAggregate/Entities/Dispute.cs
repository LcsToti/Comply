using SalesService.Domain.Aggregates.SaleAggregate.Enums;
using SalesService.Domain.Aggregates.SaleAggregate.Exceptions;

namespace SalesService.Domain.Aggregates.SaleAggregate.Entities;

public class Dispute
{
    public Guid Id { get; private set; }
    public Guid? AdminId { get; private set; }
    public DisputeStatus Status { get; private set; }
    public DisputeResolutionStatus? ResolutionStatus { get; private set; }
    public string Reason { get; private set; }
    public string? Resolution { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? ResolvedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    
    private Dispute(Guid id, DisputeStatus status, string reason, DateTime createdAt)
    {
        Id = id;
        Status = status;
        Reason = reason;
        CreatedAt = createdAt;
        ExpiresAt = createdAt.AddDays(30);
    }
    private Dispute(){}
    public static Dispute Load(
        Guid id, 
        Guid? adminId, 
        DisputeStatus status, 
        DisputeResolutionStatus? resolutionStatus, 
        string reason,
        string? resolution,
        DateTime createdAt,
        DateTime? updatedAt,
        DateTime? resolvedAt,
        DateTime? expiresAt
        )
    {
        return new Dispute()
        {
            Id = id,
            AdminId = adminId,
            Status = status,
            ResolutionStatus = resolutionStatus,
            Reason = reason,
            Resolution = resolution,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
            ResolvedAt = resolvedAt,
            ExpiresAt = expiresAt
        };
    }

    internal static Dispute Open(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new InvalidDisputeException("Reason cannot be null or empty.");
        
        return new Dispute(Guid.NewGuid(), DisputeStatus.Pending, reason, DateTime.UtcNow);
    }
    internal void AssignAdmin(Guid adminId)
    {
        if (Status != DisputeStatus.Pending)
            throw new InvalidDisputeException("Only pending disputes can move to analysis.");
        if (AdminId.HasValue)
            throw new InvalidDisputeException("Dispute already has an assigned admin.");
        
        AdminId = adminId;
        UpdatedAt = DateTime.UtcNow;      
        Status = DisputeStatus.InAnalysis;
    }
    internal void Close(DisputeResolutionStatus resolutionStatus, string resolution)
    {
        if (Status != DisputeStatus.InAnalysis)
            throw new InvalidDisputeException("Only disputes in analysis can be closed.");
        if (string.IsNullOrWhiteSpace(resolution))
            throw new InvalidDisputeException("Resolution cannot be null or empty.");

        ResolutionStatus = resolutionStatus;
        Resolution = resolution;
        Status = DisputeStatus.Closed;
        ResolvedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}