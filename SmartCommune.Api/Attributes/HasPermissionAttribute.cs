using Microsoft.AspNetCore.Authorization;

namespace SmartCommune.Api.Attributes;

public sealed class HasPermissionAttribute(string permissionName)
        : AuthorizeAttribute(policy: permissionName)
{
}