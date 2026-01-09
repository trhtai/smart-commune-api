using SmartCommune.Domain.Common.Models;

namespace SmartCommune.Domain.DossierAggregate.ValueObjects;

public sealed class DossierAttachmentId : ValueObject
{
    private DossierAttachmentId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; private set; }

    public static DossierAttachmentId CreateUnique()
    {
        return new DossierAttachmentId(Guid.CreateVersion7());
    }

    public static DossierAttachmentId Create(Guid value)
    {
        return new DossierAttachmentId(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}