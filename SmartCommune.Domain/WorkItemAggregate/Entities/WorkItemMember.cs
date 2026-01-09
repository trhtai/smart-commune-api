using SmartCommune.Domain.UserAggregate;
using SmartCommune.Domain.UserAggregate.ValueObjects;
using SmartCommune.Domain.WorkItemAggregate.ValueObjects;

namespace SmartCommune.Domain.WorkItemAggregate.Entities;

public sealed class WorkItemMember
{
#pragma warning disable CS8618
    private WorkItemMember()
    {
    }
#pragma warning restore CS8618

    private WorkItemMember(
        WorkItemId workItemId,
        ApplicationUserId memberId,
        string displayAlias,
        DateTime joinedAt)
    {
        WorkItemId = workItemId;
        MemberId = memberId;
        DisplayAlias = displayAlias;
        JoinedAt = joinedAt;
    }

    public WorkItemId WorkItemId { get; private set; }
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

    internal static WorkItemMember Create(
        WorkItemId workItemId,
        ApplicationUserId memberId,
        string displayAlias,
        DateTime joinedAt)
    {
        return new WorkItemMember(
            workItemId,
            memberId,
            displayAlias,
            joinedAt);
    }
}