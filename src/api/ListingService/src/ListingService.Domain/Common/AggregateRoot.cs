namespace ListingService.Domain.Common;

public abstract class AggregateRoot
{
    public Guid Id { get; protected set; }

    protected AggregateRoot(Guid id)
    {
        Id = id;
    }

    protected AggregateRoot() { }
}