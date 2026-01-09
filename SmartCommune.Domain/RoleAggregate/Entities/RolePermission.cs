using SmartCommune.Domain.PermissionAggregate;
using SmartCommune.Domain.PermissionAggregate.ValueObjects;
using SmartCommune.Domain.RoleAggregate.ValueObjects;

namespace SmartCommune.Domain.RoleAggregate.Entities;

public sealed class RolePermission
{
#pragma warning disable CS8618
    private RolePermission()
    {
    }
#pragma warning restore CS8618

    private RolePermission(RoleId roleId, PermissionId permissionId)
    {
        RoleId = roleId;
        PermissionId = permissionId;
    }

    public RoleId RoleId { get; private set; }
    public PermissionId PermissionId { get; private set; }

    public Permission Permission { get; private set; } = null!;

    internal static RolePermission Create(RoleId roleId, PermissionId permissionId)
    {
        return new RolePermission(roleId, permissionId);
    }
}