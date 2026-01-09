using SmartCommune.Domain.Common.Models;

namespace SmartCommune.Domain.UserAggregate.ValueObjects;

public sealed class ApplicationUserId : ValueObject
{
    private ApplicationUserId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; private set; }

    public static ApplicationUserId CreateUnique()
    {
        return new ApplicationUserId(Guid.CreateVersion7());
    }

    public static ApplicationUserId Create(Guid value)
    {
        return new ApplicationUserId(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}