using SmartCommune.Domain.Common.Models;

namespace SmartCommune.Domain.UserAggregate.ValueObjects;

public sealed class RefreshTokenId : ValueObject
{
    private RefreshTokenId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; private set; }

    public static RefreshTokenId CreateUnique()
    {
        return new RefreshTokenId(Guid.CreateVersion7());
    }

    public static RefreshTokenId Create(Guid value)
    {
        return new RefreshTokenId(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}