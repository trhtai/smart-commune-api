using SmartCommune.Domain.Common.Models;

namespace SmartCommune.Domain.WorkspaceAggregate.ValueObjects;

public sealed class WorkspaceId : ValueObject
{
    private WorkspaceId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; private set; }

    public static WorkspaceId CreateUnique()
    {
        return new WorkspaceId(Guid.CreateVersion7());
    }

    public static WorkspaceId Create(Guid value)
    {
        return new WorkspaceId(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}