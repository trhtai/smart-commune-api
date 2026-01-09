using SmartCommune.Domain.Common.Models;

namespace SmartCommune.Domain.MenuItemAggregate.ValueObjects;

public sealed class MenuItemId : ValueObject
{
    private MenuItemId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; private set; }

    public static MenuItemId CreateUnique()
    {
        return new MenuItemId(Guid.CreateVersion7());
    }

    public static MenuItemId Create(Guid value)
    {
        return new MenuItemId(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}