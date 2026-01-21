namespace SmartCommune.Constracts.Manage.MenuItems;

public record CreateMenuItemRequest(
    Guid? ParentId,
    string Label,
    string Type,
    string Path,
    string Icon,
    string ActiveIcon,
    int SortOrder,
    List<string>? CheckRoutes,
    List<string>? RelatedPaths,
    List<Guid> PermissionIds);