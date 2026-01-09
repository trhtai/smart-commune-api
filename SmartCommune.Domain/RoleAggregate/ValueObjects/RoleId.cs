using SmartCommune.Domain.Common.Models;

namespace SmartCommune.Domain.RoleAggregate.ValueObjects;

public sealed class RoleId : ValueObject
{
    private RoleId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; private set; }

    public static RoleId CreateUnique()
    {
        return new RoleId(Guid.CreateVersion7());
    }

    public static RoleId Create(Guid value)
    {
        return new RoleId(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}