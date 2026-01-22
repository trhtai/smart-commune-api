using SmartCommune.Constracts.User.MenuItems;

namespace SmartCommune.Constracts.Manage.MenuItems;

public record MenuItemsResponse(
    Guid Id,
    string Title,
    string? Icon,
    string? ActiveIcon,
    string? To,
    string Type,
    int OrderIndex,
    Guid? ParentId,
    List<string> CheckRoutes,
    List<string> RelatedPaths,
    List<Guid> AssignedPermissionIds,
    List<MenuItemResponse>? Children);