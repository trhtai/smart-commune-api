using SmartCommune.Domain.Common.Models;

namespace SmartCommune.Domain.NotificationAggregate.ValueObjects;

public sealed class NotificationId : ValueObject
{
    private NotificationId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; private set; }

    public static NotificationId CreateUnique()
    {
        return new NotificationId(Guid.CreateVersion7());
    }

    public static NotificationId Create(Guid value)
    {
        return new NotificationId(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}