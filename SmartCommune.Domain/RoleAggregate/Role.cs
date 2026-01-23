using SmartCommune.Domain.Common.Models;
using SmartCommune.Domain.PermissionAggregate.ValueObjects;
using SmartCommune.Domain.RoleAggregate.Entities;
using SmartCommune.Domain.RoleAggregate.ValueObjects;

namespace SmartCommune.Domain.RoleAggregate;

public sealed class Role : AggregateRoot<RoleId>
{
    private readonly List<RolePermission> _permissions = [];

#pragma warning disable CS8618
    private Role()
    {
    }
#pragma warning restore CS8618

    private Role(
        RoleId roleId,
        string code,
        string name)
        : base(roleId)
    {
        Code = code;
        Name = name;
    }

    public string Code { get; private set; }
    public string Name { get; private set; }

    public IReadOnlyCollection<RolePermission> Permissions => _permissions.AsReadOnly();

    public static Role Create(
        string code,
        string name)
    {
        var role = new Role(
            RoleId.CreateUnique(),
            code,
            name);

        return role;
    }

    /// <summary>
    /// Thêm quyền vào Role.
    /// </summary>
    /// <param name="permissionId">Permission Id.</param>
    public void GrantPermission(PermissionId permissionId)
    {
        if (!_permissions.Any(p => p.PermissionId == permissionId))
        {
            _permissions.Add(RolePermission.Create(Id, permissionId));
        }
    }

    /// <summary>
    /// Xóa quyền khỏi Role.
    /// </summary>
    /// <param name="permissionId">Permission Id.</param>
    public void RemovePermission(PermissionId permissionId)
    {
        var permission = _permissions.FirstOrDefault(x => x.PermissionId == permissionId);
        if (permission != null)
        {
            _permissions.Remove(permission);
        }
    }
}