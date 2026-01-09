using SmartCommune.Domain.Common.Models;

namespace SmartCommune.Domain.PlanAggregate.ValueObjects;

public sealed class PlanEvaluationId : ValueObject
{
    private PlanEvaluationId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; private set; }

    public static PlanEvaluationId CreateUnique()
    {
        return new PlanEvaluationId(Guid.CreateVersion7());
    }

    public static PlanEvaluationId Create(Guid value)
    {
        return new PlanEvaluationId(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}