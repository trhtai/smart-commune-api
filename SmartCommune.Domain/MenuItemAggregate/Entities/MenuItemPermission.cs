using SmartCommune.Domain.MenuItemAggregate.ValueObjects;
using SmartCommune.Domain.PermissionAggregate;
using SmartCommune.Domain.PermissionAggregate.ValueObjects;

namespace SmartCommune.Domain.MenuItemAggregate.Entities;

public sealed class MenuItemPermission
{
#pragma warning disable CS8618
    private MenuItemPermission()
    {
    }
#pragma warning restore CS8618

    private MenuItemPermission(MenuItemId menuItemId, PermissionId permissionId)
    {
        MenuItemId = menuItemId;
        PermissionId = permissionId;
    }

    public MenuItemId MenuItemId { get; private set; }
    public PermissionId PermissionId { get; private set; }

    public Permission Permission { get; private set; } = null!;

    internal static MenuItemPermission Create(MenuItemId menuItemId, PermissionId permissionId)
    {
        return new MenuItemPermission(menuItemId, permissionId);
    }
}