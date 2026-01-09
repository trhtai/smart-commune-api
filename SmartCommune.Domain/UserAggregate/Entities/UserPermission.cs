using SmartCommune.Domain.PermissionAggregate;
using SmartCommune.Domain.PermissionAggregate.ValueObjects;
using SmartCommune.Domain.UserAggregate.ValueObjects;

namespace SmartCommune.Domain.UserAggregate.Entities;

public sealed class UserPermission
{
#pragma warning disable CS8618
    private UserPermission()
    {
    }
#pragma warning restore CS8618

    private UserPermission(ApplicationUserId userId, PermissionId permissionId)
    {
        UserId = userId;
        PermissionId = permissionId;
    }

    public ApplicationUserId UserId { get; private set; }
    public PermissionId PermissionId { get; private set; }

    public Permission Permission { get; private set; } = null!;

    internal static UserPermission Create(ApplicationUserId userId, PermissionId permissionId)
    {
        return new UserPermission(userId, permissionId);
    }
}