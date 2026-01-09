using SmartCommune.Domain.WorkItemAggregate;
using SmartCommune.Domain.WorkItemAggregate.ValueObjects;
using SmartCommune.Domain.WorkspaceAggregate.ValueObjects;

namespace SmartCommune.Domain.WorkspaceAggregate.Entities;

public sealed class WorkItemCollection
{
#pragma warning disable CS8618
    private WorkItemCollection()
    {
    }
#pragma warning restore CS8618

    private WorkItemCollection(
        WorkspaceId workspaceId,
        WorkItemId workItemId,
        DateTime addedAt)
    {
        WorkspaceId = workspaceId;
        WorkItemId = workItemId;
        AddedAt = addedAt;
    }

    public WorkspaceId WorkspaceId { get; private set; }
    public WorkItemId WorkItemId { get; private set; }
    public DateTime AddedAt { get; private set; }

    public WorkItem Item { get; private set; } = null!;

    internal static WorkItemCollection Create(
        WorkspaceId workspaceId,
        WorkItemId workItemId,
        DateTime addedAt)
    {
        return new WorkItemCollection(
            workspaceId,
            workItemId,
            addedAt);
    }
}