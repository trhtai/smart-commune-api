using SmartCommune.Domain.Common.Models;
using SmartCommune.Domain.PermissionAggregate.ValueObjects;

namespace SmartCommune.Domain.PermissionAggregate;

public sealed class Permission : AggregateRoot<PermissionId>
{
#pragma warning disable CS8618
    private Permission()
    {
    }
#pragma warning restore CS8618

    private Permission(
        PermissionId permissionId,
        string code,
        string name)
        : base(permissionId)
    {
        Code = code;
        Name = name;
    }

    public string Code { get; private set; }
    public string Name { get; private set; }

    public static Permission Create(
        string code,
        string name)
    {
        var permission = new Permission(
            PermissionId.CreateUnique(),
            code,
            name);

        return permission;
    }
}