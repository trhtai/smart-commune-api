using SmartCommune.Domain.Common.Models;
using SmartCommune.Domain.UserAggregate;
using SmartCommune.Domain.UserAggregate.ValueObjects;
using SmartCommune.Domain.WorkItemAggregate.ValueObjects;

namespace SmartCommune.Domain.WorkItemAggregate.Entities;

public sealed class WorkItemComment : Entity<WorkItemCommentId>
{
#pragma warning disable CS8618
    private WorkItemComment()
    {
    }
#pragma warning restore CS8618

    private WorkItemComment(
        WorkItemCommentId id,
        WorkItemId workItemId,
        ApplicationUserId userId,
        string content,
        DateTime createdAt,
        WorkItemCommentId? parentCommentId)
        : base(id)
    {
        WorkItemId = workItemId;
        UserId = userId;
        Content = content;
        CreatedAt = createdAt;
        ParentId = parentCommentId;
    }

    /// <summary>
    /// Người bình luận.
    /// </summary>
    public ApplicationUserId UserId { get; private set; }
    public WorkItemId WorkItemId { get; private set; }
    public string Content { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public WorkItemCommentId? ParentId { get; private set; }

    public ApplicationUser User { get; private set; } = null!;

    public static WorkItemComment Create(
        WorkItemId workItemId,
        ApplicationUserId userId,
        string content,
        DateTime createdAt,
        WorkItemCommentId? parentCommentId)
    {
        return new WorkItemComment(
            WorkItemCommentId.CreateUnique(),
            workItemId,
            userId,
            content,
            createdAt,
            parentCommentId);
    }
}