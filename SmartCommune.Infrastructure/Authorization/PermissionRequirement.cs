using Microsoft.AspNetCore.Authorization;

namespace SmartCommune.Infrastructure.Authorization;

public class PermissionRequirement(string permissionName) : IAuthorizationRequirement
{
    public string PermissionName { get; } = permissionName;
}