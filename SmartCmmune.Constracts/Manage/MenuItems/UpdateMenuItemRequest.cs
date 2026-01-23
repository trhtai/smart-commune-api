namespace SmartCommune.Constracts.Manage.MenuItems;

public record UpdateMenuItemRequest(
    Guid? ParentId,
    string Title,
    string Type,
    string To,
    string Icon,
    string ActiveIcon,
    int SortOrder,
    List<string>? CheckRoutes,
    List<string>? RelatedPaths,
    List<Guid> PermissionIds);