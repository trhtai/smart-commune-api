using SmartCommune.Domain.Common.Models;

namespace SmartCommune.Domain.NotificationAggregate.ValueObjects;

public sealed class NotificationAttachmentId : ValueObject
{
    private NotificationAttachmentId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; private set; }

    public static NotificationAttachmentId CreateUnique()
    {
        return new NotificationAttachmentId(Guid.CreateVersion7());
    }

    public static NotificationAttachmentId Create(Guid value)
    {
        return new NotificationAttachmentId(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}