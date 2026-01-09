using SmartCommune.Domain.Common.Models;

namespace SmartCommune.Domain.WorkItemAggregate.ValueObjects;

public sealed class WorkItemReportId : ValueObject
{
    private WorkItemReportId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; private set; }

    public static WorkItemReportId CreateUnique()
    {
        return new WorkItemReportId(Guid.CreateVersion7());
    }

    public static WorkItemReportId Create(Guid value)
    {
        return new WorkItemReportId(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}