using SmartCommune.Domain.Common.Models;
using SmartCommune.Domain.Common.ValueObjects;
using SmartCommune.Domain.PlanAggregate;
using SmartCommune.Domain.PlanAggregate.ValueObjects;
using SmartCommune.Domain.UserAggregate;
using SmartCommune.Domain.UserAggregate.ValueObjects;
using SmartCommune.Domain.WorkItemAggregate.Entities;
using SmartCommune.Domain.WorkItemAggregate.ValueObjects;

namespace SmartCommune.Domain.WorkItemAggregate;

public sealed class WorkItem : AggregateRoot<WorkItemId>
{
    private readonly List<WorkItemAttachment> _attachments = [];
    private readonly List<WorkItemMember> _members = [];

#pragma warning disable CS8618
    private WorkItem()
    {
    }
#pragma warning restore CS8618

    private WorkItem(
        WorkItemId workItemId,
        string title,
        string description,
        string expectedResult,
        WorkItemStatus status,
        WorkItemPriority priority,
        int progress,
        int workload,
        DateRange timeline,
        DateTime createdAt,
        WorkItemId? parentId,
        ApplicationUserId createdById)
        : base(workItemId)
    {
        Title = title;
        Description = description;
        ExpectedResult = expectedResult;
        Status = status;
        Priority = priority;
        Progress = progress;
        Workload = workload;
        Timeline = timeline;
        CreatedAt = createdAt;
        ParentId = parentId;
        CreatedById = createdById;
    }

    public string Title { get; private set; }
    public string Description { get; private set; }
    public string ExpectedResult { get; private set; }
    public WorkItemStatus Status { get; private set; }
    public WorkItemPriority Priority { get; private set; }
    public int Progress { get; private set; }
    public int Workload { get; private set; }
    public DateRange Timeline { get; private set; }
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Đã báo trước khi sắp hết hạn chưa.
    /// </summary>
    public bool IsNotifiedBeforeDeadline { get; set; }

    /// <summary>
    /// Đã báo trễ hạn chưa.
    /// </summary>
    public bool IsNotifiedExpired { get; set; }

    public WorkItemId? ParentId { get; private set; }
    public ApplicationUserId CreatedById { get; private set; }
    public PlanId? PlanId { get; private set; }

    public ApplicationUser CreatedBy { get; private set; } = null!;
    public Plan? Plan { get; private set; } = null!;
    public IReadOnlyCollection<WorkItemAttachment> Attachments => _attachments.AsReadOnly();
    public IReadOnlyCollection<WorkItemMember> Members => _members.AsReadOnly();

    public static WorkItem Create(
        string title,
        string description,
        string expectedResult,
        WorkItemStatus status,
        WorkItemPriority priority,
        int progress,
        int workload,
        DateRange timeline,
        DateTime createdAt,
        WorkItemId? parentId,
        ApplicationUserId createdById)
    {
        return new WorkItem(
            WorkItemId.CreateUnique(),
            title,
            description,
            expectedResult,
            status,
            priority,
            progress,
            workload,
            timeline,
            createdAt,
            parentId,
            createdById);
    }
}