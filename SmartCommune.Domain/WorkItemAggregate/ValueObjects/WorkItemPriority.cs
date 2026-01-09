using Ardalis.SmartEnum;

namespace SmartCommune.Domain.WorkItemAggregate.ValueObjects;

public sealed class WorkItemPriority : SmartEnum<WorkItemPriority>
{
    public static readonly WorkItemPriority Low = new("Low", 1, "Thấp");
    public static readonly WorkItemPriority Medium = new("Medium", 2, "Trung bình");
    public static readonly WorkItemPriority High = new("High", 3, "Cao");
    public static readonly WorkItemPriority Urgent = new("Urgent", 4, "Khẩn cấp");

    private WorkItemPriority(string name, int value, string title)
        : base(name, value)
    {
        Title = title;
    }

    public string Title { get; }
}