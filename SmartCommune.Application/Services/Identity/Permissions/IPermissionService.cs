using SmartCommune.Domain.RoleAggregate.ValueObjects;

namespace SmartCommune.Application.Services.Identity.Permissions;

public interface IPermissionService
{
    Task<HashSet<string>> GetPermissionsAsync(RoleId roleId, CancellationToken cancellationToken = default);
    Task ClearCacheAsync(RoleId roleId, CancellationToken cancellationToken = default);
}