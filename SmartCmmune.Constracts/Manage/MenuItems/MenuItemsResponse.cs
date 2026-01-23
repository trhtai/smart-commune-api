namespace SmartCommune.Constracts.Manage.MenuItems;

public record MenuItemsResponse(
    Guid Id,
    string Title,
    string? Icon,
    string? ActiveIcon,
    string? To,
    string Type,
    List<string> CheckRoutes,
    List<string> RelatedPaths,
    List<Guid> AssignedPermissionIds,
    List<MenuItemsResponse>? Children);