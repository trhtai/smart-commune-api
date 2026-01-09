using SmartCommune.Domain.Common.Models;

namespace SmartCommune.Domain.PlanAggregate.ValueObjects;

public sealed class PlanId : ValueObject
{
    private PlanId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; private set; }

    public static PlanId CreateUnique()
    {
        return new PlanId(Guid.CreateVersion7());
    }

    public static PlanId Create(Guid value)
    {
        return new PlanId(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}