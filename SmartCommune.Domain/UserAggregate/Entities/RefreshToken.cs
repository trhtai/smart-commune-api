using SmartCommune.Domain.Common.Models;
using SmartCommune.Domain.UserAggregate.ValueObjects;

namespace SmartCommune.Domain.UserAggregate.Entities;

public sealed class RefreshToken : Entity<RefreshTokenId>
{
#pragma warning disable CS8618
    private RefreshToken()
    {
    }
#pragma warning restore CS8618

    private RefreshToken(
        RefreshTokenId refreshTokenId,
        string token,
        DateTime expires,
        DateTime created,
        ApplicationUserId userId)
        : base(refreshTokenId)
    {
        Token = token;
        Expires = expires;
        Created = created;
        UserId = userId;
    }

    public string Token { get; private set; }
    public DateTime Expires { get; private set; }
    public DateTime Created { get; private set; }
    public DateTime? Revoked { get; private set; }
    public ApplicationUserId UserId { get; private set; }

    /// <summary>
    /// Nếu ngày hiện tại lớn hơn hoặc bằng ngày hết hạn thì token đã hết hạn.
    /// </summary>
    /// <param name="now">Ngày hiện tại.</param>
    /// <returns>Trả về true nếu token đã hết hạn.</returns>
    public bool IsExpired(DateTime now)
    {
        return now >= Expires;
    }

    /// <summary>
    /// Kiểm tra xem token có còn hoạt động hay không.
    /// </summary>
    /// <returns>Trả về true nếu token còn hoạt động.</returns>
    public bool IsActive()
    {
        return Revoked == null;
    }

    internal static RefreshToken Create(
        string token,
        DateTime expires,
        DateTime created,
        ApplicationUserId userId)
    {
        return new(
            RefreshTokenId.CreateUnique(),
            token,
            expires,
            created,
            userId);
    }

    internal void Revoke(DateTime revokedAt)
    {
        if (Revoked == null)
        {
            Revoked = revokedAt;
        }
    }
}