using SmartCommune.Domain.Common.Models;

namespace SmartCommune.Domain.DossierAggregate.ValueObjects;

public sealed class DossierId : ValueObject
{
    private DossierId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; private set; }

    public static DossierId CreateUnique()
    {
        return new DossierId(Guid.CreateVersion7());
    }

    public static DossierId Create(Guid value)
    {
        return new DossierId(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}