using SmartCommune.Domain.Common.Models;

namespace SmartCommune.Domain.PermissionAggregate.ValueObjects;

public sealed class PermissionId : ValueObject
{
    private PermissionId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; private set; }

    public static PermissionId CreateUnique()
    {
        return new PermissionId(Guid.CreateVersion7());
    }

    public static PermissionId Create(Guid value)
    {
        return new PermissionId(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}