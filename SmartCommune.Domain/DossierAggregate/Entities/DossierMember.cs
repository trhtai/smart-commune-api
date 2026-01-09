using SmartCommune.Domain.DossierAggregate.ValueObjects;
using SmartCommune.Domain.UserAggregate;
using SmartCommune.Domain.UserAggregate.ValueObjects;

namespace SmartCommune.Domain.DossierAggregate.Entities;

public sealed class DossierMember
{
#pragma warning disable CS8618
    private DossierMember()
    {
    }
#pragma warning restore CS8618

    private DossierMember(
        DossierId dossierId,
        ApplicationUserId memberId,
        string displayAlias,
        DateTime joinedAt)
    {
        DossierId = dossierId;
        MemberId = memberId;
        DisplayAlias = displayAlias;
        JoinedAt = joinedAt;
    }

    public DossierId DossierId { get; private set; }
    public ApplicationUserId MemberId { get; private set; }

    /// <summary>
    /// Tên hiển thị tùy biến (User thích đặt là "Sếp", "Lính" gì tùy thích).
    /// </summary>
    public string DisplayAlias { get; private set; }

    /// <summary>
    /// Ngày tham gia.
    /// </summary>
    public DateTime JoinedAt { get; private set; }

    public ApplicationUser Member { get; private set; } = null!;

    internal static DossierMember Create(
        DossierId dossierId,
        ApplicationUserId memberId,
        string displayAlias,
        DateTime joinedAt)
    {
        return new DossierMember(
            dossierId,
            memberId,
            displayAlias,
            joinedAt);
    }
}