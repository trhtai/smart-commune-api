using SmartCommune.Domain.Common.Models;

namespace SmartCommune.Domain.WorkItemAggregate.ValueObjects;

public sealed class WorkItemId : ValueObject
{
    private WorkItemId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; private set; }

    public static WorkItemId CreateUnique()
    {
        return new WorkItemId(Guid.CreateVersion7());
    }

    public static WorkItemId Create(Guid value)
    {
        return new WorkItemId(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}