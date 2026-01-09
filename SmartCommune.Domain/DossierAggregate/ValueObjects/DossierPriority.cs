using Ardalis.SmartEnum;

namespace SmartCommune.Domain.DossierAggregate.ValueObjects;

public sealed class DossierPriority : SmartEnum<DossierPriority>
{
    public static readonly DossierPriority Low = new("Low", 1, "Thấp");
    public static readonly DossierPriority Medium = new("Medium", 2, "Trung bình");
    public static readonly DossierPriority High = new("High", 3, "Cao");
    public static readonly DossierPriority Urgent = new("Urgent", 4, "Khẩn cấp");

    private DossierPriority(string name, int value, string title)
        : base(name, value)
    {
        Title = title;
    }

    public string Title { get; }
}