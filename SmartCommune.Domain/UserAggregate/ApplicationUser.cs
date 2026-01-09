using SmartCommune.Domain.Common.Models;
using SmartCommune.Domain.PermissionAggregate.ValueObjects;
using SmartCommune.Domain.RoleAggregate.ValueObjects;
using SmartCommune.Domain.UserAggregate.Entities;
using SmartCommune.Domain.UserAggregate.ValueObjects;

namespace SmartCommune.Domain.UserAggregate;

public sealed class ApplicationUser : AggregateRoot<ApplicationUserId>
{
    private readonly List<UserPermission> _permissions = [];

#pragma warning disable CS8618
    private ApplicationUser()
    {
    }
#pragma warning restore CS8618

    private ApplicationUser(
        ApplicationUserId appUserId,
        string userName,
        PasswordHash passwordHash,
        string fullName,
        DateTime createdAt,
        RoleId roleId)
        : base(appUserId)
    {
        UserName = userName;
        PasswordHash = passwordHash;
        FullName = fullName;
        IsActived = true;
        CreatedAt = createdAt;
        DisableAt = null;
        RoleId = roleId;
    }

    public string UserName { get; private set; }
    public PasswordHash PasswordHash { get; private set; }
    public string FullName { get; private set; }
    public bool IsActived { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? DisableAt { get; private set; }
    public RoleId RoleId { get; private set; }

    public IReadOnlyCollection<UserPermission> Permissions => _permissions.AsReadOnly();

    public static ApplicationUser Create(
        string userName,
        string rawPassword,
        string fullName,
        DateTime createdAt,
        RoleId roleId)
    {
        var user = new ApplicationUser(
            ApplicationUserId.CreateUnique(),
            userName,
            PasswordHash.Create(rawPassword),
            fullName,
            createdAt,
            roleId);

        return user;
    }

    /// <summary>
    /// Thêm quyền vào User.
    /// </summary>
    /// <param name="permissionId">Permission Id.</param>
    public void GrantPermission(PermissionId permissionId)
    {
        if (!_permissions.Any(p => p.PermissionId == permissionId))
        {
            _permissions.Add(UserPermission.Create(Id, permissionId));
        }
    }

    /// <summary>
    /// Xóa quyền khỏi User.
    /// </summary>
    /// <param name="permissionId">Permission Id.</param>
    public void RevokePermission(PermissionId permissionId)
    {
        var permission = _permissions.FirstOrDefault(x => x.PermissionId == permissionId);
        if (permission != null)
        {
            _permissions.Remove(permission);
        }
    }
}