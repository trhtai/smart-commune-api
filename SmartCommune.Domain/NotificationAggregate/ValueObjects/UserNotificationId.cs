using SmartCommune.Domain.Common.Models;

namespace SmartCommune.Domain.NotificationAggregate.ValueObjects;

public sealed class UserNotificationId : ValueObject
{
    private UserNotificationId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; private set; }

    public static UserNotificationId CreateUnique()
    {
        return new UserNotificationId(Guid.CreateVersion7());
    }

    public static UserNotificationId Create(Guid value)
    {
        return new UserNotificationId(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}