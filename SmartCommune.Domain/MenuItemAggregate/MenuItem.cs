using SmartCommune.Domain.Common.Models;
using SmartCommune.Domain.MenuItemAggregate.Entities;
using SmartCommune.Domain.MenuItemAggregate.ValueObjects;

namespace SmartCommune.Domain.MenuItemAggregate;

public class MenuItem : AggregateRoot<MenuItemId>
{
    private readonly List<MenuItem> _children = [];
    private readonly List<MenuItemPermission> _permissions = [];

#pragma warning disable CS8618
    private MenuItem()
    {
    }
#pragma warning restore CS8618

    private MenuItem(
        MenuItemId menuId,
        string label,
        int sortOrder,
        MenuItemConfig config,
        MenuItemId? parentId)
        : base(menuId)
    {
        Label = label;
        SortOrder = sortOrder;
        Config = config;
        ParentId = parentId;
    }

    public string Label { get; private set; }
    public int SortOrder { get; private set; }
    public MenuItemConfig Config { get; private set; }
    public MenuItemId? ParentId { get; private set; }

    public IReadOnlyCollection<MenuItem> Children => _children.AsReadOnly();
    public IReadOnlyCollection<MenuItemPermission> Permissions => _permissions.AsReadOnly();

    public static MenuItem Create(
        string label,
        int sortOrder,
        MenuItemConfig config,
        MenuItemId? parentId)
    {
        var menu = new MenuItem(
            MenuItemId.CreateUnique(),
            label,
            sortOrder,
            config,
            parentId);

        return menu;
    }
}