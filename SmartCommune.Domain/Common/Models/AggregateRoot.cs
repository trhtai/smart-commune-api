namespace SmartCommune.Domain.Common.Models;

public abstract class AggregateRoot<TId> : Entity<TId>
    where TId : notnull
{
    protected AggregateRoot()
    {
        // For EF Core or other ORMs that require a parameterless constructor.
    }

    protected AggregateRoot(TId id)
        : base(id)
    {
    }
}