using SmartCommune.Domain.RoleAggregate.ValueObjects;

namespace SmartCommune.Application.Services.Identity.MenuItems;

public interface IMenuItemService
{
    Task<List<MenuItemResult>> GetMenuAsync(RoleId roleId, CancellationToken cancellationToken = default);
    Task ClearCacheAsync(RoleId roleId, CancellationToken cancellationToken = default);
}