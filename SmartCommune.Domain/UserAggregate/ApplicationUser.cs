using SmartCommune.Domain.Common.Models;
using SmartCommune.Domain.PermissionAggregate.ValueObjects;
using SmartCommune.Domain.RoleAggregate.ValueObjects;
using SmartCommune.Domain.UserAggregate.Entities;
using SmartCommune.Domain.UserAggregate.ValueObjects;

namespace SmartCommune.Domain.UserAggregate;

public sealed class ApplicationUser : AggregateRoot<ApplicationUserId>
{
    private readonly List<UserPermission> _permissions = [];
    private readonly List<RefreshToken> _refreshTokens = [];

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
        SecurityStamp = Guid.CreateVersion7();
    }

    public string UserName { get; private set; }
    public PasswordHash PasswordHash { get; private set; }
    public string FullName { get; private set; }
    public bool IsActived { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? DisableAt { get; private set; }
    public RoleId RoleId { get; private set; }
    public Guid SecurityStamp { get; private set; }

    public IReadOnlyCollection<UserPermission> Permissions => _permissions.AsReadOnly();
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

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

    /// <summary>
    /// Thêm Refresh Token mới cho User với số ngày hết hạn được chỉ định.
    /// </summary>
    /// <param name="token">Refresh token.</param>
    /// <param name="expiresDays">Số ngày hết hạn.</param>
    /// <param name="now">Ngày hiện tại.</param>
    public void AddRefreshToken(string token, int expiresDays, DateTime now)
    {
        var refreshToken = RefreshToken.Create(
            token,
            now.AddDays(expiresDays),
            now,
            Id);
        _refreshTokens.Add(refreshToken);
    }

    /// <summary>
    /// Thu hồi (revoke) một refresh token của người dùng.
    /// </summary>
    /// <param name="token">Refresh token cần thu hồi.</param>
    /// <param name="now">Ngày hiện tại.</param>
    /// <returns>Trả về <c>true</c> nếu refresh token tồn tại và đang còn hiệu lực, và đã được thu hồi thành công;
    /// trả về <c>false</c> nếu token không tồn tại, đã hết hạn hoặc đã bị thu hồi trước đó.</returns>
    /// <remarks>
    /// Việc thu hồi refresh token thường được sử dụng khi:
    /// <list type="bullet">
    /// <item><description>Người dùng đăng xuất.</description></item>
    /// <item><description>Phát hiện hành vi bảo mật bất thường.</description></item>
    /// <item><description>Thực hiện refresh token rotation.</description></item>
    /// </list>
    /// Phương thức này không xóa token khỏi hệ thống mà chỉ đánh dấu thời điểm thu hồi để phục vụ kiểm soát và audit.
    /// </remarks>
    public bool RevokeRefreshToken(string token, DateTime now)
    {
        var existingToken = _refreshTokens.FirstOrDefault(t => t.Token == token);

        if (existingToken is null || !existingToken.IsActive(now))
        {
            return false;
        }

        existingToken.Revoke(now);

        return true;
    }

    /// <summary>
    /// Thực hiện luân chuyển (rotate) refresh token cho người dùng.
    /// </summary>
    /// <param name="oldToken">
    /// Refresh token cũ cần được thu hồi.
    /// </param>
    /// <param name="newToken">
    /// Refresh token mới sẽ được cấp thay thế.
    /// </param>
    /// <param name="expiresDays">
    /// Số ngày hết hạn của refresh token mới.
    /// </param>
    /// <param name="now">
    /// Ngày hiện tại.
    /// </param>
    /// <remarks>
    /// Refresh token rotation là cơ chế bảo mật, trong đó:
    /// <list type="number">
    /// <item><description>Refresh token cũ được thu hồi.</description></item>
    /// <item><description>Refresh token mới được tạo và gắn với người dùng.</description></item>
    /// </list>
    /// Cơ chế này giúp giảm thiểu rủi ro khi refresh token bị lộ và đảm bảo mỗi refresh token chỉ được sử dụng một lần.
    /// </remarks>
    public void RotateRefreshToken(string oldToken, string newToken, int expiresDays, DateTime now)
    {
        RevokeRefreshToken(oldToken, now);
        AddRefreshToken(newToken, expiresDays, now);
    }

    /// <summary>
    /// Gọi hàm này khi: Đổi mật khẩu, Đổi quyền, User bị khóa...
    /// </summary>
    public void RefreshSecurityStamp()
    {
        SecurityStamp = Guid.NewGuid();
    }
}