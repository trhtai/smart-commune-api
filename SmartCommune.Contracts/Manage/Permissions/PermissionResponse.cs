namespace SmartCommune.Contracts.Manage.Permissions;

public record PermissionResponse(
    Guid Id,
    string Code,
    string Name);