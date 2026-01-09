using SmartCommune.Domain.Common.Models;

namespace SmartCommune.Domain.WorkItemAggregate.ValueObjects;

public sealed class WorkItemReportAttachmentId : ValueObject
{
    private WorkItemReportAttachmentId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; private set; }

    public static WorkItemReportAttachmentId CreateUnique()
    {
        return new WorkItemReportAttachmentId(Guid.CreateVersion7());
    }

    public static WorkItemReportAttachmentId Create(Guid value)
    {
        return new WorkItemReportAttachmentId(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}