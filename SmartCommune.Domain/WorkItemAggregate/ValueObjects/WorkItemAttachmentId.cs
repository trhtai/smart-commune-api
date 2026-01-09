using SmartCommune.Domain.Common.Models;

namespace SmartCommune.Domain.WorkItemAggregate.ValueObjects;

public sealed class WorkItemAttachmentId : ValueObject
{
    private WorkItemAttachmentId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; private set; }

    public static WorkItemAttachmentId CreateUnique()
    {
        return new WorkItemAttachmentId(Guid.CreateVersion7());
    }

    public static WorkItemAttachmentId Create(Guid value)
    {
        return new WorkItemAttachmentId(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}