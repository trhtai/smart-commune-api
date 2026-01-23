namespace SmartCommune.Application.Services.Manage.MenuItems.Common;

public record MenuItemManageResult(
    Guid Id,
    string Label,
    string? Icon,
    string? ActiveIcon,
    string? RouterLink,
    string Type,
    int OrderIndex,
    Guid? ParentId,
    List<string> CheckRoutes,
    List<string> RelatedPaths,
    List<Guid> AssignedPermissionIds,
    List<MenuItemManageResult>? Children);