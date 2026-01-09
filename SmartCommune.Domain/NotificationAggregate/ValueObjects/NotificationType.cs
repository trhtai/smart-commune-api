using Ardalis.SmartEnum;

namespace SmartCommune.Domain.NotificationAggregate.ValueObjects;

public sealed class NotificationType : SmartEnum<NotificationType>
{
    public static readonly NotificationType Info = new("Info", 1);
    public static readonly NotificationType Urgent = new("Urgent", 2);

    private NotificationType(string name, int value)
        : base(name, value)
    {
    }

    public bool IsPriority()
    {
        return Value == Urgent.Value;
    }
}