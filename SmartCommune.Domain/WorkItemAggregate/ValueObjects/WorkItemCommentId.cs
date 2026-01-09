using SmartCommune.Domain.Common.Models;

namespace SmartCommune.Domain.WorkItemAggregate.ValueObjects;

public sealed class WorkItemCommentId : ValueObject
{
    private WorkItemCommentId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; private set; }

    public static WorkItemCommentId CreateUnique()
    {
        return new WorkItemCommentId(Guid.CreateVersion7());
    }

    public static WorkItemCommentId Create(Guid value)
    {
        return new WorkItemCommentId(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}